namespace FTN.Constellation.Routing
{
    public class RouterStatistics
    {
        public long DeliveredMessages;
        public long UndeliveredMessages;
        public long UnmatchedMessages;
        public long MatchedMessages;
        public long TotalSyncDeliveryTime;
        public long TotalAsyncDeliveryTime;
        public long TotalMatchingTime;
        public long TotalMatchAttempts { get { return MatchedMessages + UnmatchedMessages; } }
        public long TotalMessagesTransmitted { get { return DeliveredMessages + UndeliveredMessages; } }
        public long AverageSyncDeliveryTime;
        public long AverageAsyncDeliveryTime;

        //stats
        //messages delivered
        //average delivery rate
        //delivery rate last minute
        //delivery rate last 10 seconds
        //delivery rate last seconds
        //current rate
        //average async delivery time
        //average sync delivery time
        //average queue depth

    }
}