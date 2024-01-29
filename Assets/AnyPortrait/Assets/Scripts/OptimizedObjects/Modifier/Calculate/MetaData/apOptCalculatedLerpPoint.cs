/*
*	Copyright (c) 2017-2023. RainyRizzle Inc. All rights reserved
*	Contact to : https://www.rainyrizzle.com/ , contactrainyrizzle@gmail.com
*
*	This file is part of [AnyPortrait].
*
*	AnyPortrait can not be copied and/or distributed without
*	the express permission of [Seungjik Lee] of [RainyRizzle team].
*
*	It is illegal to download files from other than the Unity Asset Store and RainyRizzle homepage.
*	In that case, the act could be subject to legal sanctions.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using AnyPortrait;

namespace AnyPortrait
{

	public class apOptCalculatedLerpPoint
	{
		// Members
		//-----------------------------------------
		public Vector2 _pos = Vector2.zero;//Float / Vector2 값을 사용하는 경우
		public int _iPos = 0;//Int 값을 사용하는 경우

		//True이면 실제 데이터가 있는 것이며, False이면 미리 다른 키로부터 보간된 가상 Point이다.
		public bool _isRealPoint = true;

		//보간 처리를 위해 Param을 저장한다. 여러개 저장 가능 (가상 Point인 경우)
		public List<apOptCalculatedResultParam.OptParamKeyValueSet> _refParams = new List<apOptCalculatedResultParam.OptParamKeyValueSet>();
		public List<float> _refWeights = new List<float>();

		//계산용 변수
		private int _nRefParams = 0;
		private apOptCalculatedResultParam.OptParamKeyValueSet _curRefPKV = null;
		private float _curRefWeight = 0f;

		public float _calculatedWeight = 0.0f;


		// Init
		//-----------------------------------------
		public apOptCalculatedLerpPoint(Vector2 vPos, bool isRealPoint)
		{
			_pos = vPos;
			_isRealPoint = isRealPoint;

			_refParams.Clear();
			_refWeights.Clear();
			_nRefParams = 0;
		}

		public apOptCalculatedLerpPoint(float fPos, bool isRealPoint)
		{
			_pos.x = fPos;
			_isRealPoint = isRealPoint;

			_refParams.Clear();
			_refWeights.Clear();
			_nRefParams = 0;
		}

		public apOptCalculatedLerpPoint(int iPos, bool isRealPoint)
		{
			_iPos = iPos;
			_isRealPoint = isRealPoint;

			_refParams.Clear();
			_refWeights.Clear();
			_nRefParams = 0;
		}


		// Functions
		//-----------------------------------------
		public void AddPoint(apOptCalculatedResultParam.OptParamKeyValueSet point, float weight)
		{
			_refParams.Add(point);
			_refWeights.Add(weight);
			_nRefParams = _refParams.Count;
		}

		public void Addpoints(apOptCalculatedLerpPoint lerpPoint, float weight)
		{
			for (int i = 0; i < lerpPoint._refParams.Count; i++)
			{
				AddPoint(lerpPoint._refParams[i], lerpPoint._refWeights[i] * weight);
			}
		}

		public void CalculateITPWeight()
		{
			for (int i = 0; i < _nRefParams; i++)
			{
				_curRefPKV = _refParams[i];
				_curRefWeight = _refWeights[i];

				_curRefPKV._isCalculated = true;
				_curRefPKV._weight += _curRefWeight * _calculatedWeight;
			}
		}


		// Get / Set
		//-----------------------------------------
	}
}