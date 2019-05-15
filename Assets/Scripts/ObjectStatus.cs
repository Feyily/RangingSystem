public enum PointState
{
    finding,
    holding,
    Placed,
    Adsorpting
}

public enum FocusState
{
    Initializing,
    Finding,
    Found,
    Absorbing,//被吸附
    Hidden//隐藏
}

public enum MeasureStatus
{
    /*公用状态*/
    Initializing,//正在检测平面
    Adding,//准备添加第一个点
    /*长度测量*/
    Length_Measuring,//正在测量长度状态（长度测量用）
    /*角度测量*/
    FirstLine_Drawing,//首条线绘制状态（角度/面积测量用）
    Angle_Measuring,//测量角度中
    /*距离测量*/
    Distance_Measuring,//正在测量距离状态

    OtherLine_Drawing,//其他线绘制状态（角度/面积测量用）
    NeedLight,
    Complete
}