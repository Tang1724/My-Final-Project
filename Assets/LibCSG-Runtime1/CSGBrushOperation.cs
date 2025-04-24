using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LibCSG1
{
    public enum Operation {
        OPERATION_UNION,
        OPERATION_INTERSECTION,
        OPERATION_SUBTRACTION,
    };

    public class CSGBrushOperation
    {
        public struct Build2DFaceCollection {
            public Dictionary<int, Build2DFaces> build2DFacesA;
            public Dictionary<int, Build2DFaces> build2DFacesB;
        }

        public CSGBrushOperation() {}

        public IEnumerator merge_brushes(Operation operation, CSGBrush brush_a, CSGBrush brush_b, CSGBrush merged_brush, float tolerance = 0.001f, Action<float> onProgress = null)
        {
            Build2DFaceCollection build2DFaceCollection;
            build2DFaceCollection.build2DFacesA = new Dictionary<int, Build2DFaces>(brush_a.faces.Length);
            build2DFaceCollection.build2DFacesB = new Dictionary<int, Build2DFaces>(brush_b.faces.Length);

            brush_a.regen_face_aabbs();
            brush_b.regen_face_aabbs();

            // 分帧执行 update_faces
            yield return update_faces_async(brush_a, brush_b, build2DFaceCollection, tolerance, progress => {
                onProgress?.Invoke(progress * 0.4f); // 前 40% 分配给交叉面处理
            });

            // Mesh 合并
            MeshMerge mesh_merge = new MeshMerge(
                brush_a.faces.Length + build2DFaceCollection.build2DFacesA.Count,
                brush_b.faces.Length + build2DFaceCollection.build2DFacesB.Count
            );

            mesh_merge.vertex_snap = tolerance;
            mesh_merge.scale_a = brush_a.obj.transform.localScale;

            // 构建 brush A 的面
            for (int i = 0; i < brush_a.faces.Length; i++) {
                if (build2DFaceCollection.build2DFacesA.ContainsKey(i)) {
                    build2DFaceCollection.build2DFacesA[i].addFacesToMesh(ref mesh_merge, false);
                } else {
                    Vector3[] points = new Vector3[3];
                    Vector2[] uvs = new Vector2[3];
                    for (int j = 0; j < 3; j++) {
                        points[j] = brush_a.faces[i].vertices[j];
                        uvs[j] = brush_a.faces[i].uvs[j];
                    }
                    mesh_merge.add_face(points, uvs, false);
                }
            }

            // 构建 brush B 的面
            for (int i = 0; i < brush_b.faces.Length; i++) {
                if (build2DFaceCollection.build2DFacesB.ContainsKey(i)) {
                    build2DFaceCollection.build2DFacesB[i].addFacesToMesh(ref mesh_merge, true);
                } else {
                    Vector3[] points = new Vector3[3];
                    Vector2[] uvs = new Vector2[3];
                    for (int j = 0; j < 3; j++) {
                        points[j] = brush_a.obj.transform.InverseTransformPoint(
                            brush_b.obj.transform.TransformPoint(brush_b.faces[i].vertices[j])
                        );
                        uvs[j] = brush_b.faces[i].uvs[j];
                    }
                    mesh_merge.add_face(points, uvs, true);
                }
            }

            Array.Clear(merged_brush.faces, 0, merged_brush.faces.Length);

            // 分帧执行布尔运算，后 60%
            yield return mesh_merge.DoOperationAsync(operation, merged_brush, 10, (progress) => {
                onProgress?.Invoke(0.4f + 0.6f * progress);
            });

            mesh_merge = null;
        }

        // 分帧的 update_faces
        private IEnumerator update_faces_async(CSGBrush brush_a, CSGBrush brush_b, Build2DFaceCollection collection, float vertex_snap, Action<float> onProgress = null)
        {
            int total = brush_a.faces.Length * brush_b.faces.Length;
            int processed = 0;

            for (int i = 0; i < brush_a.faces.Length; i++) {
                for (int j = 0; j < brush_b.faces.Length; j++) {
                    if (brush_a.faces[i].aabb.intersects_inclusive(brush_b.faces[j].aabb)) {
                        update_faces(ref brush_a, i, ref brush_b, j, ref collection, vertex_snap);
                    }

                    processed++;
                    if (processed % 50 == 0) {
                        onProgress?.Invoke((float)processed / total);
                        yield return null;
                    }
                }
            }

            onProgress?.Invoke(1f);
        }

        // 保持不变：用于检测交叉面并插入
        void update_faces(ref CSGBrush brush_a, int face_idx_a, ref CSGBrush brush_b, int face_idx_b, ref Build2DFaceCollection collection, float vertex_snap)
        {
            Vector3[] vertices_a = {
                brush_a.faces[face_idx_a].vertices[0],
                brush_a.faces[face_idx_a].vertices[1],
                brush_a.faces[face_idx_a].vertices[2]
            };

            Vector3[] vertices_b = {
                brush_a.obj.transform.InverseTransformPoint(brush_b.obj.transform.TransformPoint(brush_b.faces[face_idx_b].vertices[0])),
                brush_a.obj.transform.InverseTransformPoint(brush_b.obj.transform.TransformPoint(brush_b.faces[face_idx_b].vertices[1])),
                brush_a.obj.transform.InverseTransformPoint(brush_b.obj.transform.TransformPoint(brush_b.faces[face_idx_b].vertices[2]))
            };

            bool has_degenerate = false;
            if (CSGBrush.is_snapable(vertices_a[0], vertices_a[1], vertex_snap) ||
                CSGBrush.is_snapable(vertices_a[0], vertices_a[2], vertex_snap) ||
                CSGBrush.is_snapable(vertices_a[1], vertices_a[2], vertex_snap)) {
                collection.build2DFacesA[face_idx_a] = new Build2DFaces();
                has_degenerate = true;
            }

            if (CSGBrush.is_snapable(vertices_b[0], vertices_b[1], vertex_snap) ||
                CSGBrush.is_snapable(vertices_b[0], vertices_b[2], vertex_snap) ||
                CSGBrush.is_snapable(vertices_b[1], vertices_b[2], vertex_snap)) {
                collection.build2DFacesB[face_idx_b] = new Build2DFaces();
                has_degenerate = true;
            }

            if (has_degenerate) return;

            int over = 0, under = 0;
            Plane plane_a = new Plane(vertices_a[0], vertices_a[1], vertices_a[2]);
            float tolerance = 0.3f;
            for (int i = 0; i < 3; i++) {
                float d = plane_a.GetDistanceToPoint(vertices_b[i]);
                if (Mathf.Abs(d) < tolerance) continue;
                if (d > 0) over++; else under++;
            }
            if (over == 3 || under == 3) return;

            over = 0; under = 0;
            Plane plane_b = new Plane(vertices_b[0], vertices_b[1], vertices_b[2]);
            for (int i = 0; i < 3; i++) {
                float d = plane_b.GetDistanceToPoint(vertices_a[i]);
                if (Mathf.Abs(d) < tolerance) continue;
                if (d > 0) over++; else under++;
            }
            if (over == 3 || under == 3) return;

            // SAT 检测
            for (int i = 0; i < 3; i++) {
                Vector3 axis_a = (vertices_a[i] - vertices_a[(i + 1) % 3]).normalized;
                for (int j = 0; j < 3; j++) {
                    Vector3 axis_b = (vertices_b[j] - vertices_b[(j + 1) % 3]).normalized;
                    Vector3 sep_axis = Vector3.Cross(axis_a, axis_b);
                    if (sep_axis == Vector3.zero) continue;
                    sep_axis.Normalize();

                    float minA = float.MaxValue, maxA = float.MinValue;
                    float minB = float.MaxValue, maxB = float.MinValue;

                    foreach (var v in vertices_a) {
                        float d = Vector3.Dot(sep_axis, v);
                        minA = Mathf.Min(minA, d);
                        maxA = Mathf.Max(maxA, d);
                    }
                    foreach (var v in vertices_b) {
                        float d = Vector3.Dot(sep_axis, v);
                        minB = Mathf.Min(minB, d);
                        maxB = Mathf.Max(maxB, d);
                    }

                    minB -= (maxA - minA) * 0.5f;
                    maxB += (maxA - minA) * 0.5f;

                    float dmin = minB - (minA + maxA) * 0.5f;
                    float dmax = maxB - (minA + maxA) * 0.5f;

                    if (dmin > CSGBrush.CMP_EPSILON || dmax < -CSGBrush.CMP_EPSILON) {
                        return;
                    }
                }
            }

            if (!collection.build2DFacesA.ContainsKey(face_idx_a)) {
                collection.build2DFacesA.Add(face_idx_a, new Build2DFaces(brush_a, face_idx_a));
            }
            collection.build2DFacesA[face_idx_a].insert(brush_b, face_idx_b, brush_a);

            if (!collection.build2DFacesB.ContainsKey(face_idx_b)) {
                collection.build2DFacesB.Add(face_idx_b, new Build2DFaces(brush_b, face_idx_b, brush_a));
            }
            collection.build2DFacesB[face_idx_b].insert(brush_a, face_idx_a);
        }
    }
}