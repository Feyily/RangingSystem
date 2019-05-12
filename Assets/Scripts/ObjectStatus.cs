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
    Absorbing//被吸附
}

public enum MeasureStatus
{
    Initializing,//正在检测平面
    Adding,//准备添加第一个点
    Length_Measuring,//正在测量长度状态（长度测量用）
    Line_Drawing,//首条线绘制状态（角度/面积测量用）
    Wating,//等待下一条线绘制（角度/面积测量用）
    Angle_Measuring,//测量角度中
    OtherLine_Drawing,//其他线绘制状态（角度/面积测量用）
    NeedLight,
    Complete
}