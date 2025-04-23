using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LibCSG1{

public class MeshMerge{
        // Use a limit to speed up bvh and limit the depth.
        static int BVH_LIMIT = 10;

        enum VISIT {
            TEST_AABB_BIT = 0,
            VISIT_LEFT_BIT = 1,
            VISIT_RIGHT_BIT = 2,
            VISIT_DONE_BIT = 3,
            VISITED_BIT_SHIFT = 29,
            NODE_IDX_MASK = (1 << VISITED_BIT_SHIFT) - 1,
            VISITED_BIT_MASK = ~NODE_IDX_MASK
        };


		public struct FaceBVH {
			public int face;
			public int left;
			public int right;
			public int next;
			public Vector3 center;
			public AABB aabb;
		};


        public struct Face {
			public bool from_b;
			public int[] points;
			public Vector2[] uvs;
		};
        

		public List<Vector3> points;  


		public List<Face> faces_a;


        private int face_from_a = 0;


		public List<Face> faces_b;
        


        private int face_from_b = 0;

		public float vertex_snap = 0.0f;


		Dictionary<Vector3, int> snap_cache;


		public Vector3 scale_a;


        public class FaceBVHCmpX : IComparer {
			int IComparer.Compare(object obj1, object obj2) {
                (int i,FaceBVH f) p_left = ((int ,FaceBVH)) obj1;
                (int i,FaceBVH f) p_right = ((int ,FaceBVH)) obj2;
				if(p_left.f.center.x == p_right.f.center.x){
                    return 0;
                }
				if(p_left.f.center.x < p_right.f.center.x){
                    return 1;
                }
                return -1;
			}
		};


        public class FaceBVHCmpY : IComparer {
			int IComparer.Compare(object obj1, object obj2) {
                (int i,FaceBVH f) p_left = ((int ,FaceBVH)) obj1;
                (int i,FaceBVH f) p_right = ((int ,FaceBVH)) obj2;
				if(p_left.f.center.y == p_right.f.center.y){
                    return 0;
                }
				if(p_left.f.center.y < p_right.f.center.y){
                    return 1;
                }
                return -1;
			}
		};


        public class FaceBVHCmpZ : IComparer {
			int IComparer.Compare(object obj1, object obj2) {
                (int i,FaceBVH f) p_left = ((int ,FaceBVH)) obj1;
                (int i,FaceBVH f) p_right = ((int ,FaceBVH)) obj2;
				if(p_left.f.center.z == p_right.f.center.z){
                    return 0;
                }
				if(p_left.f.center.z < p_right.f.center.z){
                    return 1;
                }
                return -1;
			}
		};


        public MeshMerge(int size_a = 0, int size_b = 0){
            points = new List<Vector3>();
            faces_a = new List<Face>(size_a);
            faces_b = new List<Face>(size_b);
            snap_cache = new Dictionary<Vector3, int>(3*(size_a+size_b));
        }


        int create_bvh(ref FaceBVH[] facebvh, ref (int i,FaceBVH f)[] id_facebvh, int from, int size, int depth, ref int r_max_depth) {
            if (depth > r_max_depth) {
                r_max_depth = depth;
            }

            if (size == 0) {
                return -1;
            }

            if (size <= BVH_LIMIT) {
                for (int i = 0; i < size - 1; i++) {
                    facebvh[id_facebvh[from + i].i].next = id_facebvh[from + i + 1].i;
                }
                return id_facebvh[from].i;
            }

            AABB aabb;
            aabb = new AABB(facebvh[id_facebvh[from].i].aabb.get_position(),facebvh[id_facebvh[from].i].aabb.get_size());
            for (int i = 1; i < size; i++) {
                aabb.merge_with(id_facebvh[from + i].f.aabb);
            }

            int li = aabb.get_longest_axis_index();

            switch (li) {
                case 0: {
                    SortArray temp = new SortArray(new FaceBVHCmpX());
                    temp.nth_element(from, from + size, from + size/2, ref id_facebvh);
                } break;

                case 1: {
                    SortArray temp = new SortArray(new FaceBVHCmpY());
                    temp.nth_element(from, from + size, from + size/2, ref id_facebvh);
                } break;

                case 2: {
                    SortArray temp = new SortArray(new FaceBVHCmpZ());
                    temp.nth_element(from, from + size, from + size/2, ref id_facebvh);
                } break;
            }

            int left = create_bvh(ref facebvh, ref id_facebvh, from, size / 2, depth + 1, ref r_max_depth);
            int right = create_bvh(ref facebvh, ref id_facebvh, from + size / 2, size - size / 2, depth + 1, ref r_max_depth);

            Array.Resize(ref facebvh, facebvh.Length + 1);
            facebvh[facebvh.Length - 1].aabb = aabb;
            facebvh[facebvh.Length - 1].center = aabb.get_center();
            facebvh[facebvh.Length - 1].face = -1;
            facebvh[facebvh.Length - 1].left = left;
            facebvh[facebvh.Length - 1].right = right;
            facebvh[facebvh.Length - 1].next = -1;

            return facebvh.Length - 1;
        }


        void add_distance(ref List<float> r_intersectionsA, ref List<float> r_intersectionsB, bool from_B, float distance) {
            List<float> intersections = from_B ? r_intersectionsB : r_intersectionsA;

            // Check if distance exists.
            foreach (float E in intersections) {
                if (Mathf.Abs(E - distance)<CSGBrush.CMP_EPSILON) {
                    return;
                }
            }

            intersections.Add(distance);
        }


        bool bvh_inside(ref FaceBVH[] facebvh, int max_depth, int bvh_first, int face_idx, bool from_faces_a) {
            Face face;
            if(from_faces_a){
                face = faces_a[face_idx];
            }
            else{
                face = faces_b[face_idx];
            }
            
            Vector3[] face_points = {
                points[face.points[0]],
                points[face.points[1]],
                points[face.points[2]]
            };
            Vector3 face_center = (face_points[0] + face_points[1] + face_points[2]) / 3.0f;
            Vector3 face_normal = new PlaneCSG(face_points[0], face_points[1], face_points[2]).normal;

            int[] stack =  new int[max_depth];
 

            List<float> intersectionsA = new List<float>();
            List<float> intersectionsB = new List<float>();

            int level = 0;
            int pos = bvh_first;
            stack[0] = pos;
            int c = stack[level];

            while (true) {
                int node = stack[level] & (int)VISIT.NODE_IDX_MASK;
                FaceBVH? current_facebvh = facebvh[node];
                bool done = false;

                switch (stack[level] >> (int)VISIT.VISITED_BIT_SHIFT) {
                    case (int)VISIT.TEST_AABB_BIT: {
                        if (((FaceBVH)current_facebvh).face >= 0) {
                            while (current_facebvh!=null) {
                                if ( ((FaceBVH)current_facebvh).aabb.intersects_ray(face_center, face_normal)) {
                                    Face current_face;
                                    if(from_faces_a){
                                        current_face = faces_b[((FaceBVH)current_facebvh).face];
                                    }
                                    else{
                                        current_face = faces_a[((FaceBVH)current_facebvh).face];
                                    }
                                    Vector3[] current_points = {
                                        points[current_face.points[0]],
                                        points[current_face.points[1]],
                                        points[current_face.points[2]]
                                    };
                                    Vector3 current_normal = new PlaneCSG(current_points[0], current_points[1], current_points[2]).normal;
                                    Vector3 intersection_point = new Vector3();

                                    // Check if faces are co-planar.
                                    if (CSGBrush.is_equal_approx(current_normal, face_normal) &&
                                            CSGBrush.is_point_in_triangle(face_center, current_points)) {
                                        // Only add an intersection if not a B face.
                                        if (!face.from_b) {
                                            add_distance(ref intersectionsA, ref intersectionsB, current_face.from_b, 0);
                                        }
                                    } else if (CSGBrush.ray_intersects_triangle(face_center, face_normal, current_points, CSGBrush.CMP_EPSILON,ref intersection_point)) {
                                        float distance = Vector3.Distance(face_center, intersection_point);
                                        add_distance(ref intersectionsA, ref intersectionsB, current_face.from_b, distance);
                                    }
                                }

                                if (((FaceBVH)current_facebvh).next != -1) {
                                    current_facebvh = facebvh[((FaceBVH)current_facebvh).next];
                                } else {
                                    current_facebvh = null;
                                }
                            }

                            stack[level] = ((int)VISIT.VISIT_DONE_BIT << (int)VISIT.VISITED_BIT_SHIFT) | node;

                        } else {
                            bool valid = ((FaceBVH)current_facebvh).aabb.intersects_ray(face_center, face_normal);

                            if (!valid) {
                                stack[level] = ((int)VISIT.VISIT_DONE_BIT << (int)VISIT.VISITED_BIT_SHIFT) | node;
                            } else {
                                stack[level] = ((int)VISIT.VISIT_LEFT_BIT << (int)VISIT.VISITED_BIT_SHIFT) | node;
                            }
                        }
                        continue;
                    }

                    case (int)VISIT.VISIT_LEFT_BIT: {
                        stack[level] = ((int)VISIT.VISIT_RIGHT_BIT << (int)VISIT.VISITED_BIT_SHIFT) | node;
                        stack[level + 1] = ((FaceBVH)current_facebvh).left | (int)VISIT.TEST_AABB_BIT;
                        level++;
                        continue;
                    }

                    case (int)VISIT.VISIT_RIGHT_BIT: {
                        stack[level] = ((int)VISIT.VISIT_DONE_BIT << (int)VISIT.VISITED_BIT_SHIFT) | node;
                        stack[level + 1] = ((FaceBVH)current_facebvh).right | (int)VISIT.TEST_AABB_BIT;
                        level++;
                        continue;
                    }

                    case (int)VISIT.VISIT_DONE_BIT: {
                        if (level == 0) {
                            done = true;
                            break;
                        } else {
                            level--;
                        }
                        continue;
                    }
                }

                if (done) {
                    break;
                }
            }
            // Inside if face normal intersects other faces an odd number of times.
            int res = (intersectionsA.Count + intersectionsB.Count) & 1;
            return res != 0;
        }


    public IEnumerator DoOperationAsync(Operation operation, CSGBrush r_merged_brush, int yieldEvery = 20, Action<float> onProgress = null)
{
    // 初始化 BVH
    FaceBVH[] facebvh_a = new FaceBVH[faces_a.Count];
    FaceBVH[] facebvh_b = new FaceBVH[faces_b.Count];
    AABB aabb_a = new AABB();
    AABB aabb_b = new AABB();
    bool first_a = true;
    bool first_b = true;

    for (int i = 0; i < faces_a.Count; i++) {
        facebvh_a[i] = new FaceBVH();
        facebvh_a[i].left = -1;
        facebvh_a[i].right = -1;
        facebvh_a[i].face = i;
        facebvh_a[i].aabb = new AABB();
        facebvh_a[i].aabb.set_position(points[faces_a[i].points[0]]);
        facebvh_a[i].aabb.expand_to(points[faces_a[i].points[1]]);
        facebvh_a[i].aabb.expand_to(points[faces_a[i].points[2]]);
        facebvh_a[i].center = facebvh_a[i].aabb.get_center();
        facebvh_a[i].aabb.grow_by(vertex_snap);
        facebvh_a[i].next = -1;
        if (first_a) { aabb_a = facebvh_a[i].aabb.Copy(); first_a = false; }
        else { aabb_a.merge_with(facebvh_a[i].aabb); }
    }

    for (int i = 0; i < faces_b.Count; i++) {
        facebvh_b[i] = new FaceBVH();
        facebvh_b[i].left = -1;
        facebvh_b[i].right = -1;
        facebvh_b[i].face = i;
        facebvh_b[i].aabb = new AABB();
        facebvh_b[i].aabb.set_position(points[faces_b[i].points[0]]);
        facebvh_b[i].aabb.expand_to(points[faces_b[i].points[1]]);
        facebvh_b[i].aabb.expand_to(points[faces_b[i].points[2]]);
        facebvh_b[i].center = facebvh_b[i].aabb.get_center();
        facebvh_b[i].aabb.grow_by(vertex_snap);
        facebvh_b[i].next = -1;
        if (first_b) { aabb_b = facebvh_b[i].aabb.Copy(); first_b = false; }
        else { aabb_b.merge_with(facebvh_b[i].aabb); }
    }

    AABB intersection_aabb = aabb_a.intersection(aabb_b);

    (int, FaceBVH)[] bvhtrvec_a = new (int, FaceBVH)[faces_a.Count];
    for (int i = 0; i < faces_a.Count; i++) {
        bvhtrvec_a[i] = (i, facebvh_a[i]);
    }

    (int, FaceBVH)[] bvhtrvec_b = new (int, FaceBVH)[faces_b.Count];
    for (int i = 0; i < faces_b.Count; i++) {
        bvhtrvec_b[i] = (i, facebvh_b[i]);
    }

    int max_depth_a = 0, max_depth_b = 0;
    create_bvh(ref facebvh_a, ref bvhtrvec_a, 0, face_from_a, 1, ref max_depth_a);
    int max_alloc_a = facebvh_a.Length;

    create_bvh(ref facebvh_b, ref bvhtrvec_b, 0, face_from_b, 1, ref max_depth_b);
    int max_alloc_b = facebvh_b.Length;

    int faces_count = 0;
    List<CSGBrush.Face> result_faces = new List<CSGBrush.Face>();

    int total = faces_a.Count + faces_b.Count;
    int processed = 0;

    switch (operation)
    {
        case Operation.OPERATION_UNION:
        case Operation.OPERATION_SUBTRACTION:
            for (int i = 0; i < faces_a.Count; i++) {
                bool skip = !intersection_aabb.intersects_inclusive(facebvh_a[i].aabb) ||
                            (!bvh_inside(ref facebvh_b, max_depth_b, max_alloc_b - 1, i, true));
                if (skip) {
                    CSGBrush.Face f = new CSGBrush.Face();
                    f.vertices = new List<Vector3> {
                        points[faces_a[i].points[0]],
                        points[faces_a[i].points[1]],
                        points[faces_a[i].points[2]]
                    };
                    f.uvs = faces_a[i].uvs;
                    result_faces.Add(f);
                }

                processed++;
                if (processed % yieldEvery == 0) {
                    onProgress?.Invoke(processed / (float)total);
                    yield return null;
                }
            }

            if (operation == Operation.OPERATION_UNION) {
                for (int i = 0; i < faces_b.Count; i++) {
                    bool skip = !intersection_aabb.intersects_inclusive(facebvh_b[i].aabb) ||
                                !bvh_inside(ref facebvh_a, max_depth_a, max_alloc_a - 1, i, false);
                    if (skip) {
                        CSGBrush.Face f = new CSGBrush.Face();
                        f.vertices = new List<Vector3> {
                            points[faces_b[i].points[0]],
                            points[faces_b[i].points[1]],
                            points[faces_b[i].points[2]]
                        };
                        f.uvs = faces_b[i].uvs;
                        result_faces.Add(f);
                    }

                    processed++;
                    if (processed % yieldEvery == 0) {
                        onProgress?.Invoke(processed / (float)total);
                        yield return null;
                    }
                }
            } else if (operation == Operation.OPERATION_SUBTRACTION) {
                for (int i = 0; i < faces_b.Count; i++) {
                    bool include = intersection_aabb.intersects_inclusive(facebvh_b[i].aabb) &&
                                   bvh_inside(ref facebvh_a, max_depth_a, max_alloc_a - 1, i, false);
                    if (include) {
                        CSGBrush.Face f = new CSGBrush.Face();
                        f.vertices = new List<Vector3> {
                            points[faces_b[i].points[1]],
                            points[faces_b[i].points[0]],
                            points[faces_b[i].points[2]]
                        };
                        f.uvs = faces_b[i].uvs;
                        result_faces.Add(f);
                    }

                    processed++;
                    if (processed % yieldEvery == 0) {
                        onProgress?.Invoke(processed / (float)total);
                        yield return null;
                    }
                }
            }
            break;

        case Operation.OPERATION_INTERSECTION:
            for (int i = 0; i < faces_a.Count; i++) {
                bool include = intersection_aabb.intersects_inclusive(facebvh_a[i].aabb) &&
                               bvh_inside(ref facebvh_b, max_depth_b, max_alloc_b - 1, i, true);
                if (include) {
                    CSGBrush.Face f = new CSGBrush.Face();
                    f.vertices = new List<Vector3> {
                        points[faces_a[i].points[0]],
                        points[faces_a[i].points[1]],
                        points[faces_a[i].points[2]]
                    };
                    f.uvs = faces_a[i].uvs;
                    result_faces.Add(f);
                }

                processed++;
                if (processed % yieldEvery == 0) {
                    onProgress?.Invoke(processed / (float)total);
                    yield return null;
                }
            }

            for (int i = 0; i < faces_b.Count; i++) {
                bool include = intersection_aabb.intersects_inclusive(facebvh_b[i].aabb) &&
                               bvh_inside(ref facebvh_a, max_depth_a, max_alloc_a - 1, i, false);
                if (include) {
                    CSGBrush.Face f = new CSGBrush.Face();
                    f.vertices = new List<Vector3> {
                        points[faces_b[i].points[0]],
                        points[faces_b[i].points[1]],
                        points[faces_b[i].points[2]]
                    };
                    f.uvs = faces_b[i].uvs;
                    result_faces.Add(f);
                }

                processed++;
                if (processed % yieldEvery == 0) {
                    onProgress?.Invoke(processed / (float)total);
                    yield return null;
                }
            }
            break;
    }

    r_merged_brush.faces = result_faces.ToArray();
    onProgress?.Invoke(1f);
    yield return null;
}


        public void add_face( Vector3[] points, Vector2[] uvs, bool from_b) {
            int[] indices = new int[3];
            for (int i = 0; i < 3; i++) {
                Vector3 vk = new Vector3();
                vk.x = Mathf.RoundToInt( ((points[i].x + vertex_snap) * 0.31234f) / vertex_snap);
                vk.y = Mathf.RoundToInt( ((points[i].y + vertex_snap) * 0.31234f) / vertex_snap);
                vk.z = Mathf.RoundToInt( ((points[i].z + vertex_snap) * 0.31234f) / vertex_snap);

                if (snap_cache.ContainsKey(vk)) {
                    indices[i] = snap_cache[vk];
                } else {
                    indices[i] = this.points.Count;
                    this.points.Add(Vector3.Scale(points[i],scale_a));
                    snap_cache.Add(vk, indices[i]);
                }
            }

            // Don't add degenerate faces.
            if (indices[0] == indices[2] || indices[0] == indices[1] || indices[1] == indices[2]) {
                return;
            }

            Face face = new Face();
            face.from_b = from_b;
            face.points = new int[3];
            face.uvs = new Vector2[3];

            for (int k = 0; k < 3; k++) {
                face.points[k] = indices[k];
                face.uvs[k] = uvs[k];
            }
            if(from_b){
                face_from_b++;
                faces_b.Add(face);
            }
            else{
                face_from_a++;
                faces_a.Add(face);
            }

        }

    }
}
