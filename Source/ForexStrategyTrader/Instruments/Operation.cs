using System;

namespace ForexStrategyBuilder
{
    public class Operation
    {
        public Operation(DateTime barOpenTime, OperationType operationType, DateTime operationTime, double operationLots,
                         double operationPrice)
        {
            BarTime = barOpenTime;
            OperationType = operationType;
            OperationTime = operationTime;
            OperationLots = operationLots;
            OperationPrice = operationPrice;
        }

        public DateTime BarTime { get; set; }
        public OperationType OperationType { get; set; }
        public double OperationLots { get; set; }
        public DateTime OperationTime { get; set; }
        public double OperationPrice { get; set; }
    }
}