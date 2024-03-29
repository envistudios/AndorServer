﻿using System;
using BEPUphysicsDemos.AlternateMovement.Character;
using BEPUutilities;
using RegionServer.Model.Interfaces;
using AndorServerCommon.MessageObjects;

namespace RegionServer.Model
{
    public class BEPUPhysics : IPhysics
    {
        private bool _dirty;
        private Position _position = new Position();
        public MoveDirection Direction { get; set; }
        private PlayerMovement _playerMovement;
        public CharacterController CharacterController { get; set; }

        #region Implementation of IPhysics

        public Vector3 WalkDirection { get; set; }
        public bool Moving { get; set; }
        public Position Position 
        {   get { return _position; }
            set { _position = value; }
        }
        public float MoveSpeed { get; set; }

        public bool Dirty 
        {
            get
            {
                if (CharacterController != null && 
                    CharacterController.Body != null)
                {
                    Position pos = CharacterController.Body.WorldTransform;
                    
                    if (_position != pos)
                    {
                        _position = pos;
                        _dirty = true;
                    }
                }

                return _dirty;
            }
            set
            {
                _dirty = value;
            }
        }

        public PlayerMovement Movement 
        {
            get
            {
                return _playerMovement;
            }
            set
            {
                _playerMovement = value;

                if (CharacterController != null && CharacterController.Body != null)
                {
                    Moving = false;
                    Direction = MoveDirection.None;

                    //var xform =
                    //    ((KinematicCharacterController) CharacterController).GetGhostObject().GetWorldTransform();
                    //xform.SetRotation(Quaternion.CreateFromAxisAngle(new Vector3(0,1,0), Util.DegToRad(_playerMovement.Facing*0.0055f)));

                    CharacterController.Body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)-Math.PI*(_playerMovement.Facing*0.0055f)/180f);

                    Vector3 forwardDirection = CharacterController.Body.WorldTransform.Forward;
                    Vector3 upDirection = CharacterController.Body.WorldTransform.Up;
                    Vector3 strafeDirection = CharacterController.Body.WorldTransform.Right;

                    forwardDirection.Normalize();
                    upDirection.Normalize();
                    strafeDirection.Normalize();

                    var moveSpeed = MoveSpeed;
                    WalkDirection = new Vector3(0,0,0);

                    if (_playerMovement.Walk)
                    {
                        moveSpeed /= 4.3f;
                    }

                    if (_playerMovement.Right < 0)
                    {
                        WalkDirection -= strafeDirection;
                        Direction |= MoveDirection.Left;
                        Moving = true;
                    }

                    if (_playerMovement.Right > 0)
                    {
                        WalkDirection += strafeDirection;
                        Direction |= MoveDirection.Right;
                        Moving = true;
                    }

                    if (_playerMovement.Forward < 0)
                    {
                        WalkDirection -= forwardDirection;
                        Direction |= MoveDirection.Backward;
                        Moving = true;
                    }

                    if (_playerMovement.Forward > 0)
                    {
                        WalkDirection += forwardDirection;
                        Direction |= MoveDirection.Forward;
                        Moving = true;
                    }

                    WalkDirection.Normalize();

                    Vector3 refVector = (WalkDirection*moveSpeed);
                    CharacterController.HorizontalMotionConstraint.MovementDirection = new Vector2(refVector.X, refVector.Z);
                }
            }
        }

        #endregion
    }
}