using System;
using System.Collections.Generic;
using System.Linq;

namespace MajorProject.Web.Helpers
{
    /// <summary>Session-local duplicate-post protection; no database claims are made here.</summary>
    [Serializable]
    public sealed class BookingSubmissionLedger
    {
        private readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
        private long _sequence;
        public string Issue(int customerID)
        {
            if (customerID <= 0)
                throw new ArgumentException("A valid customer is required.");
            while (_entries.Count >= 40)
            {
                string oldest = _entries.OrderBy(pair => pair.Value.Sequence).First().Key;
                _entries.Remove(oldest);
            }

            string token = Guid.NewGuid().ToString("N");
            _entries.Add(token, new Entry { CustomerID = customerID, Sequence = ++_sequence });
            return token;
        }

        public bool TryGetResult(string token, int customerID, out int jobID)
        {
            jobID = 0;
            Entry entry;
            if (string.IsNullOrEmpty(token) || !_entries.TryGetValue(token, out entry) || entry.CustomerID != customerID)
                return false;
            jobID = entry.JobID;
            return true;
        }

        public void Complete(string token, int customerID, int jobID)
        {
            int existing;
            if (jobID <= 0 || !TryGetResult(token, customerID, out existing))
                throw new InvalidOperationException("This booking form has expired.");
            if (existing > 0 && existing != jobID)
                throw new InvalidOperationException("This booking has already been completed.");
            _entries[token].JobID = jobID;
        }

        [Serializable]
        private sealed class Entry
        {
            public int CustomerID;
            public int JobID;
            public long Sequence;
        }
    }
}
