using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Entities
{
    public class AIBase
    {
        protected readonly WorldManager world;
        protected readonly Mob mob;
        protected readonly PathFinder pathFinder;

        // AI参数
        public float SightRange { get; set; } = 16.0f;
        public float HearingRange { get; set; } = 8.0f;
        public float ReactionTime { get; set; } = 0.5f;
        public float MemoryDuration { get; set; } = 30.0f;

        // 目标记忆
        protected Entity rememberedTarget;
        protected float memoryTimer;

        // 巡逻点
        protected List<Vector3> patrolPoints;
        protected int currentPatrolIndex;

        // 状态计时器
        protected float stateTimer;
        protected float actionTimer;

        public AIBase(WorldManager world, Mob mob)
        {
            this.world = world;
            this.mob = mob;
            pathFinder = new PathFinder(world);
            patrolPoints = new List<Vector3>();
        }

        public virtual void Update(float deltaTime)
        {
            // 更新记忆
            UpdateMemory(deltaTime);

            // 感知周围
            SenseSurroundings();

            // 决策
            MakeDecision(deltaTime);
        }

        protected virtual void UpdateMemory(float deltaTime)
        {
            if (rememberedTarget != null)
            {
                memoryTimer -= deltaTime;
                if (memoryTimer <= 0 || !rememberedTarget.IsAlive())
                {
                    rememberedTarget = null;
                }
            }
        }

        protected virtual void SenseSurroundings()
        {
            // 视觉检测
            if (mob.IsHostile)
            {
                Entity nearestPlayer = FindNearestPlayer(SightRange);
                if (nearestPlayer != null)
                {
                    if (HasLineOfSight(nearestPlayer))
                    {
                        rememberedTarget = nearestPlayer;
                        memoryTimer = MemoryDuration;
                    }
                }
            }
        }

        protected virtual void MakeDecision(float deltaTime)
        {
            stateTimer -= deltaTime;

            if (rememberedTarget != null && rememberedTarget.IsAlive())
            {
                float distance = mob.GetDistanceTo(rememberedTarget);

                if (mob.IsHostile)
                {
                    if (distance <= mob.AttackRange)
                    {
                        mob.CurrentState = AIState.Attack;
                        mob.Target = rememberedTarget;
                    }
                    else if (distance <= mob.FollowRange)
                    {
                        mob.CurrentState = AIState.Chase;
                        mob.Target = rememberedTarget;
                    }
                }
                else if (mob.IsPassive)
                {
                    if (distance < 8.0f)
                    {
                        mob.CurrentState = AIState.Flee;
                        mob.Target = rememberedTarget;
                    }
                }
            }
        }

        public virtual Entity FindNearestPlayer(float range)
        {
            // 简化：假设玩家在(0, 100, 0)附近
            // 实际应该从EntityManager获取所有玩家
            return null;
        }

        public virtual List<Entity> FindEntitiesInRange(float range)
        {
            List<Entity> entities = new List<Entity>();
            // 实际应该从EntityManager获取
            return entities;
        }

        public virtual bool HasLineOfSight(Entity target)
        {
            Vector3 start = mob.GetEyePosition();
            Vector3 end = target.GetEyePosition();
            Vector3 direction = end - start;
            float distance = direction.Length;
            direction.Normalize();

            // 简单的射线检测
            for (float t = 0; t < distance; t += 0.5f)
            {
                Vector3 point = start + direction * t;
                int x = (int)Math.Floor(point.X);
                int y = (int)Math.Floor(point.Y);
                int z = (int)Math.Floor(point.Z);

                ushort block = world.GetBlock(x, y, z);
                if (IsOpaqueBlock(block))
                {
                    return false;
                }
            }

            return true;
        }

        protected bool IsOpaqueBlock(ushort blockId)
        {
            if (blockId == GameConstants.BLOCK_AIR) return false;
            if (blockId == GameConstants.BLOCK_GLASS) return false;
            if (blockId == GameConstants.BLOCK_WATER_STILL) return false;
            if (blockId == GameConstants.BLOCK_WATER_FLOWING) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            return info != null && info.IsSolid && !info.IsTransparent;
        }

        public virtual bool CanReach(Vector3 target)
        {
            return pathFinder.FindPath(mob.Position, target) != null;
        }

        public virtual List<Vector3> GetPathTo(Vector3 target)
        {
            return pathFinder.FindPath(mob.Position, target);
        }

        public virtual void SetPatrolPoints(List<Vector3> points)
        {
            patrolPoints = points;
            currentPatrolIndex = 0;
        }

        public virtual Vector3 GetNextPatrolPoint()
        {
            if (patrolPoints.Count == 0) return Vector3.Zero;

            Vector3 point = patrolPoints[currentPatrolIndex];
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            return point;
        }

        public virtual void ReactToDamage(Entity attacker)
        {
            if (mob.IsNeutral || mob.IsHostile)
            {
                rememberedTarget = attacker;
                memoryTimer = MemoryDuration;
                mob.Target = attacker;
                mob.CurrentState = AIState.Chase;
            }
            else if (mob.IsPassive)
            {
                rememberedTarget = attacker;
                memoryTimer = 5.0f;
                mob.Target = attacker;
                mob.CurrentState = AIState.Flee;
            }
        }

        public virtual void OnHeardSound(Vector3 soundPosition, float volume)
        {
            if (mob.IsHostile && volume > 0.5f)
            {
                float distance = mob.GetDistanceTo(soundPosition);
                if (distance < HearingRange * volume)
                {
                    // 调查声音来源
                    mob.WanderTarget = soundPosition;
                    mob.CurrentState = AIState.Wander;
                }
            }
        }
    }

    // ========================================
    // 寻路算法 (A*)
    // ========================================
    public class PathFinder
    {
        private readonly WorldManager world;
        private readonly int maxIterations = 1000;
        private readonly float stepCost = 1.0f;
        private readonly float diagonalCost = 1.414f;

        public PathFinder(WorldManager world)
        {
            this.world = world;
        }

        public List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            Vector3i startNode = new Vector3i(
                (int)Math.Floor(start.X),
                (int)Math.Floor(start.Y),
                (int)Math.Floor(start.Z)
            );

            Vector3i endNode = new Vector3i(
                (int)Math.Floor(end.X),
                (int)Math.Floor(end.Y),
                (int)Math.Floor(end.Z)
            );

            if (!IsWalkable(endNode))
            {
                // 尝试找到终点附近可行走的点
                endNode = FindNearestWalkable(endNode);
                if (endNode == Vector3i.Zero) return null;
            }

            // A* 算法
            Dictionary<Vector3i, PathNode> openSet = new Dictionary<Vector3i, PathNode>();
            Dictionary<Vector3i, PathNode> closedSet = new Dictionary<Vector3i, PathNode>();

            PathNode startPathNode = new PathNode
            {
                Position = startNode,
                G = 0,
                H = Heuristic(startNode, endNode),
                Parent = null
            };
            startPathNode.F = startPathNode.G + startPathNode.H;

            openSet[startNode] = startPathNode;

            int iterations = 0;

            while (openSet.Count > 0 && iterations < maxIterations)
            {
                iterations++;

                // 找到F值最小的节点
                PathNode current = GetLowestFNode(openSet);

                if (current.Position == endNode)
                {
                    return ReconstructPath(current);
                }

                openSet.Remove(current.Position);
                closedSet[current.Position] = current;

                // 遍历邻居
                foreach (Vector3i neighbor in GetNeighbors(current.Position))
                {
                    if (closedSet.ContainsKey(neighbor)) continue;
                    if (!IsWalkable(neighbor)) continue;

                    float tentativeG = current.G + stepCost;

                    if (!openSet.ContainsKey(neighbor) || tentativeG < openSet[neighbor].G)
                    {
                        PathNode neighborNode = new PathNode
                        {
                            Position = neighbor,
                            G = tentativeG,
                            H = Heuristic(neighbor, endNode),
                            Parent = current
                        };
                        neighborNode.F = neighborNode.G + neighborNode.H;

                        openSet[neighbor] = neighborNode;
                    }
                }
            }

            return null; // 没有找到路径
        }

        private PathNode GetLowestFNode(Dictionary<Vector3i, PathNode> openSet)
        {
            PathNode lowest = null;
            foreach (PathNode node in openSet.Values)
            {
                if (lowest == null || node.F < lowest.F)
                {
                    lowest = node;
                }
            }
            return lowest;
        }

        private List<Vector3> ReconstructPath(PathNode endNode)
        {
            List<Vector3> path = new List<Vector3>();
            PathNode current = endNode;

            while (current != null)
            {
                path.Add(new Vector3(
                    current.Position.X + 0.5f,
                    current.Position.Y,
                    current.Position.Z + 0.5f
                ));
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        private List<Vector3i> GetNeighbors(Vector3i node)
        {
            List<Vector3i> neighbors = new List<Vector3i>
            {
                new Vector3i(node.X + 1, node.Y, node.Z),
                new Vector3i(node.X - 1, node.Y, node.Z),
                new Vector3i(node.X, node.Y, node.Z + 1),
                new Vector3i(node.X, node.Y, node.Z - 1),
                new Vector3i(node.X, node.Y + 1, node.Z),
                new Vector3i(node.X, node.Y - 1, node.Z)
            };

            return neighbors;
        }

        private bool IsWalkable(Vector3i node)
        {
            // 检查脚的位置是否为空，头的位置是否为空，脚下是否有固体
            ushort feet = world.GetBlock(node.X, node.Y, node.Z);
            ushort head = world.GetBlock(node.X, node.Y + 1, node.Z);
            ushort ground = world.GetBlock(node.X, node.Y - 1, node.Z);

            bool feetEmpty = feet == GameConstants.BLOCK_AIR || feet == GameConstants.BLOCK_WATER_STILL;
            bool headEmpty = head == GameConstants.BLOCK_AIR || head == GameConstants.BLOCK_WATER_STILL;
            bool groundSolid = IsSolid(ground);

            return feetEmpty && headEmpty && groundSolid;
        }

        private bool IsSolid(ushort blockId)
        {
            if (blockId == GameConstants.BLOCK_AIR) return false;
            if (blockId == GameConstants.BLOCK_WATER_STILL) return false;
            if (blockId == GameConstants.BLOCK_LAVA_STILL) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            return info != null && info.IsSolid;
        }

        private Vector3i FindNearestWalkable(Vector3i target)
        {
            for (int radius = 1; radius <= 5; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dz = -radius; dz <= radius; dz++)
                        {
                            Vector3i candidate = new Vector3i(
                                target.X + dx,
                                target.Y + dy,
                                target.Z + dz
                            );
                            if (IsWalkable(candidate))
                            {
                                return candidate;
                            }
                        }
                    }
                }
            }
            return Vector3i.Zero;
        }

        private float Heuristic(Vector3i a, Vector3i b)
        {
            // 曼哈顿距离
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }
    }

    public class PathNode
    {
        public Vector3i Position;
        public float G; // 从起点到当前节点的成本
        public float H; // 从当前节点到终点的估计成本
        public float F; // G + H
        public PathNode Parent;
    }
}
