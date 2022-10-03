using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
		Camera referenceCamera;

		public enum Axis { up, down, left, right, forward, back };
		public bool reverseFace = false;
		public Axis axis = Axis.up;

		// 선택한 axis(축)에 따른 vector 방향을 리턴함
		public Vector3 GetAxis(Axis refAxis)
		{
			switch (refAxis)
			{
				case Axis.down:
					return Vector3.down;
				case Axis.forward:
					return Vector3.forward;
				case Axis.back:
					return Vector3.back;
				case Axis.left:
					return Vector3.left;
				case Axis.right:
					return Vector3.right;
			}

			// default 값
			return Vector3.up;
		}

		void Awake()
		{
			// 참조된 카메라가 없으면, 메인 카메라를 쓰도록 함
			if (!referenceCamera)
				referenceCamera = Camera.main;
		}
		// frame의 모든 움직임이 완료된 후, 카메라 방향을 조정함 
		void LateUpdate()
		{
			// 카메라 기준으로 object를 회전시킴
			Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
			Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis(axis);
			transform.LookAt(targetPos, targetOrientation);
		}
	}
}
