using System;
using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public class EntityStatus
    {
        public EntityStatus(StatStruct baseStats)
        {
            this.BaseStats = baseStats;
        }

        public StatStruct BaseStats { get; }

        public StatStruct WriteCurrentStats { get; private set; }

        public StatStruct ReadCurrentStats { get; private set; }

        public List<Status> ActiveStatuses { get; } = new List<Status>();

        public void ResetToBaseStats()
        {
            this.BaseStats.Health = this.BaseStats.MaxHealth;
            this.WriteCurrentStats = new StatStruct(this.BaseStats);
        }

        public void Step(GameEntity entity, GameState gameState)
        {
            var newCurrentStats = new StatStruct(this.WriteCurrentStats);

            foreach (var status in this.ActiveStatuses)
            {
                status.Duration--;
                status.ApplyTo(newCurrentStats);

                var tickEffects = status.Tick(entity, gameState);
                if (tickEffects != null)
                {
                    gameState.HandleGameEvents(tickEffects);
                }
            }

            this.ActiveStatuses.RemoveAll(s => s.Duration <= 0);

            this.ReadCurrentStats = newCurrentStats;
        }

        public void ApplyStatus(Status status)
        {
            var activeStatus = this.ActiveStatuses.FirstOrDefault(s => s.GetType() == status.GetType() && s.Applyor.Id == status.Applyor.Id);

            if (activeStatus != null)
            {
                activeStatus.Stack(status);
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
            this.DamageTakenMultiplier = copyFrom.DamageTakenMultiplier;
            this.Strength = copyFrom.Strength;
        }

        public int MaxHealth { get; set; }

        public int Health { get; set; } = 1;

        public float MoveSpeed { get; set; }

        public int GlobalCooldown { get; set; } = 150;

        public float DamageTakenMultiplier { get; set; } = 1;

        public int Strength { get; set; } = 1;
    }
}
