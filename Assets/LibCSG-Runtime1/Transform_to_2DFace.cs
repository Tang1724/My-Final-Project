using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LibCSG1{


public class Transform_to_2DFace
{


    private Vector3 Basisl1 = new Vector3(1, 0, 0);
    private Vector3 Basisl2 = new Vector3(0, 1, 0);
    private Vector3 Basisl3 = new Vector3(0, 0, 1);


    private Vector3 position = new Vector3(0,0,0);
    

    public Transform_to_2DFace(){
    }


    public Transform_to_2DFace(Vector3 pos, Vector3 basisl1, Vector3 basisl2, Vector3 basisl3){
        this.Basisl1 = basisl1;
        this.Basisl2 = basisl2;
        this.Basisl3 = basisl3;
        this.position = pos;
    }


    public void basis_set_column(int col, Vector3 valeur){
        this.Basisl1[col] = valeur[0];
        this.Basisl2[col] = valeur[1];
        this.Basisl3[col] = valeur[2];
    }


    public Vector3 basis_get_column(int col){
        return new Vector3(Basisl1[col], Basisl2[col], Basisl3[col]);
    }


    public Vector3 xform(Vector3 vector)  {
	return new Vector3(
			Vector3.Dot(Basisl1, vector) + position.x,
			Vector3.Dot(Basisl2 ,vector) + position.y,
			Vector3.Dot(Basisl3 ,vector) + position.z);
    }

  
    private Vector3 Basis_xform(Vector3 vector) {
        return new Vector3(
                Vector3.Dot(Basisl1, vector),
                Vector3.Dot(Basisl2, vector),
                Vector3.Dot(Basisl3, vector));
    }


    public void affine_invert() {
        this.Basis_invert();
        this.position = this.Basis_xform(-position);
    }


    private void Basis_invert() {
        float[] co = {
            cofac(ref Basisl2, 1, ref Basisl3, 2), cofac(ref Basisl2, 2, ref Basisl3, 0), cofac(ref Basisl2, 0, ref Basisl3, 1)
        };
        float det = Basisl1[0] * co[0] + Basisl1[1] * co[1] + Basisl1[2] * co[2];

        float s = 1.0f / det;

        this.Set_basis(co[0] * s, cofac(ref Basisl1, 2, ref Basisl3, 1) * s, cofac(ref Basisl1, 1, ref Basisl2, 2) * s,
                co[1] * s, cofac(ref Basisl1, 0, ref Basisl3, 2) * s, cofac(ref Basisl1, 2, ref Basisl2, 0) * s,
                co[2] * s, cofac(ref Basisl1, 1, ref Basisl3, 0) * s, cofac(ref Basisl1, 0, ref Basisl2, 1) * s);
    }


    private float cofac(ref Vector3 row1, int col1, ref Vector3 row2, int col2) {
	    return row1[col1] * row2[col2] - row1[col2] * row2[col1];
    }

    public void Set_basis(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz) {
		this.Basisl1[0] = xx;
		this.Basisl1[1] = xy;
		this.Basisl1[2] = xz;
		this.Basisl2[0] = yx;
		this.Basisl2[1] = yy;
		this.Basisl2[2] = yz;
		this.Basisl3[0] = zx;
		this.Basisl3[1] = zy;
		this.Basisl3[2] = zz;
	}


    public void Set_position(Vector3 pos) {
        this.position.Set(pos.x, pos.y, pos.z);
    }


    public Transform_to_2DFace affine_inverse() {
        Transform_to_2DFace res = new Transform_to_2DFace(this.position, Basisl1, Basisl2, Basisl3);
        res.affine_invert();
        return res;
    }
}
}