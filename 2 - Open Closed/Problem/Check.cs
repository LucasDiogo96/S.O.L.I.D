using System;

namespace OCP.Problem
{
    public class Check
    {
        public int Id { get; set; }
        public CheckTypeEnum Type { get; set; }
        public string Justification { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
    }
}
