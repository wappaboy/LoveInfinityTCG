using System;
using System.Collections.Generic;
using UnityEngine;

public class CalendarDatePivots : SingletonMonoBehaviour<CalendarDatePivots>
{
	[SerializeField] int _startYear;
	[SerializeField] int _StartMonth;
	[SerializeField] int _startDate;
	DateTime _startDateTIme;
	[SerializeField] List<RectTransform> _datePivots = new List<RectTransform>();
	readonly bool _dontDestroyOnLoad = false;
	protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;
	
	//生成された約束の日を保持
	public DateTime _promiseDate { get; set; }

	void Start()
	{
		//すべての子オブジェクトを取得して、_datePivotsに追加する
		foreach (Transform child in transform)
		{
			foreach (Transform c in child)
			{
				_datePivots.Add(c.GetComponent<RectTransform>());
			}
		}

		//_startDateTimeを設定する
		_startDateTIme = new DateTime(_startYear, _StartMonth, _startDate);

		//Debug.Log(GetDatePivot(new DateTime(2023, 7, 7)).gameObject.name);
	}

	//_startDateTimeの日付を_datePivotsの最初のTransfomrと対応させ、引数のDateTimeに対応する_datePivotsのTransformを返す
	public RectTransform GetDatePivot(DateTime dateTime)
	{
		var dateSpan = dateTime - _startDateTIme;
		var datePivotNum = dateSpan.Days;
		Debug.Log(datePivotNum);
		return _datePivots[datePivotNum];
	}
}