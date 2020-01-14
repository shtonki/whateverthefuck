using System;
using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public class EntityStatus
    {
        public StatStruct BaseStats { get; } = new StatStruct();

        public StatStruct WriteCurrentStats { get; private set; }

        public StatStruct ReadCurrentStats { get; private set; }

        private List<Status> ActiveStatuses { get; } = new List<Status>();

        public void ResetToBaseStats()
        {
            Logging.Log(this.BaseStats.MaxHealth);
            this.BaseStats.Health = this.BaseStats.MaxHealth;
            this.WriteCurrentStats = new StatStruct(this.BaseStats);
        }

        public void Step()
        {
            var newCurrentStats = new StatStruct(this.WriteCurrentStats);

            foreach (var status in this.ActiveStatuses)
            {
                status.Duration--;
                status.ApplyTo(newCurrentStats);
            }

            this.ActiveStatuses.RemoveAll(s => s.Duration <= 0);

            this.ReadCurrentStats = newCurrentStats;
        }

        public void ApplyStatus(Status status)
        {
            var activeStatus = this.ActiveStatuses.FirstOrDefault(s => s.GetType() == status.GetType());

            if (activeStatus != null)
            {
                activeStatus.Stacks += status.Stacks;
                activeStatus.Duration = Math.Max(activeStatus.Duration, status.Duration);
            }
            else
            {
                this.ActiveStatuses.Add(status);
            }
        }
    }

    public class StatStruct
    {
        public StatStruct()
        {
        }

        public StatStruct(StatStruct copyFrom)
        {
            this.MaxHealth = copyFrom.MaxHealth;
            this.Health = copyFrom.Health;
            this.MoveSpeed = copyFrom.MoveSpeed;
        }

        public int MaxHealth { get; set; }

        public int Health { get; set; }

        public float MoveSpeed { get; set; }
    }
}
