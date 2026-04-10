namespace IceTask_Two.Models
{
    /// <summary>
    /// Simple rules for which status you can move to next (edit here if your brief differs).
    /// </summary>
    public static class ContractWorkflow
    {
        public static bool IsValidChange(ContractStatus from, ContractStatus to)
        {
            if (from == to)
            {
                return false;
            }

            // Draft -> can activate or put on hold
            if (from == ContractStatus.Draft)
            {
                if (to == ContractStatus.Active) return true;
                if (to == ContractStatus.OnHold) return true;
                return false;
            }

            // Active -> can pause or mark expired
            if (from == ContractStatus.Active)
            {
                if (to == ContractStatus.OnHold) return true;
                if (to == ContractStatus.Expired) return true;
                return false;
            }

            // On hold -> can go back to draft, resume active, or expire
            if (from == ContractStatus.OnHold)
            {
                if (to == ContractStatus.Draft) return true;
                if (to == ContractStatus.Active) return true;
                if (to == ContractStatus.Expired) return true;
                return false;
            }

            // Expired -> start a new cycle as draft (e.g. renewal)
            if (from == ContractStatus.Expired)
            {
                if (to == ContractStatus.Draft) return true;
                return false;
            }

            return false;
        }

        // Used by the Details page to show only the buttons that are allowed
        public static List<ContractStatus> GetNextStatuses(ContractStatus current)
        {
            var options = new List<ContractStatus>();
            foreach (ContractStatus status in Enum.GetValues(typeof(ContractStatus)))
            {
                if (IsValidChange(current, status))
                {
                    options.Add(status);
                }
            }
            return options;
        }
    }
}
