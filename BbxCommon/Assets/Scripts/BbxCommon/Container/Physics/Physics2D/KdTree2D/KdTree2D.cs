﻿using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon.Container
{
    // Operate kd-tree through this class externally.
    public class KdTree2D : PooledObject
    {
        internal KdTree2DNode Root;
        private HashSet<ColliderItem2D> m_ColliderItemSet = new HashSet<ColliderItem2D>();

        public void Init()
        {
            Root = ObjectPool<KdTree2DNode>.Alloc();
            Root.InitLeaf(this, null);
        }

        public void Add(ColliderItem2D item)
        {
            if (m_ColliderItemSet.Contains(item))
            {
                return;
            }
            Root.AddColliderItem(item);
        }

        public void Remove(ColliderItem2D item)
        {
            if (m_ColliderItemSet.Contains(item) == false)
            {
                return;
            }
            Root.RemoveColliderItem(item);
        }

        /// <summary>
        /// Update the item's position.
        /// </summary>
        public void UpdateItem(ColliderItem2D item, Vector2 newPosition)
        {
            if (Root.NodeBelongsToChanged(item, newPosition))
            {
                Root.RemoveColliderItem(item);
                item.SetPosition(newPosition);
                Root.AddColliderItem(item);
            }
            item.SetPosition(newPosition);
        }

        public bool CheckCollisionCircle(CircleData2D circle)
        {
            var item = ObjectPool<ColliderItem2DCircle>.Alloc();
            item.Init(circle.Position, circle.Radius);
            var res = Root.CheckCollision(item);
            item.Collect();
            return res;
        }

        public bool CheckCollisionCapsule(CapsuleBaselineData2D capsule)
        {
            return Root.CheckCollisionCapsule(capsule);
        }

        public bool CheckCollisionLine(Vector2 startPos, Vector2 endPos)
        {
            return Root.CheckCollisionCapsule(new CapsuleBaselineData2D(startPos, endPos, 0));
        }

        public void DestroyTree()
        {
            Root.DestroyTree();
        }

        public override void OnCollect()
        {
            base.OnCollect();
            DestroyTree();
        }
    }
}
