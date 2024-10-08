using System;
using System.Collections.Generic;

namespace UbiArt {
	public class MemoryData_RaymanLegendsVITA : IMemoryData {
		#region Data
		public static readonly Dictionary<Type, uint> EditorSizes = new Dictionary<Type, uint> {
			// Main
			[typeof(StringID)] = 8,
			[typeof(Path)] = 140, // 132 + StringID
			[typeof(LocalizedPath)] = 144, // Path + LocId
			[typeof(LocalisationId)] = 4,
			[typeof(ColorInteger)] = 4,
			[typeof(string)] = 20,
			[typeof(Vec2d)] = 8,
			[typeof(Vec3d)] = 12,
			[typeof(Vec4d)] = 16,
			[typeof(Color)] = 16,
			[typeof(Angle)] = 4,
			[typeof(AngleAmount)] = 4,
			[typeof(ObjectId)] = 8,
			[typeof(ObjectRef)] = 4,
			[typeof(PathRef)] = 4,
			[typeof(Platform)] = 4,
			[typeof(SoundGUID)] = 4,
			[typeof(Volume)] = 4,

			// From game data
			[typeof(ITF.FriseConfig)] = 1432,
			[typeof(ITF.CollisionFrieze)] = 44,
			[typeof(ITF.FillConfig)] = 40,
			[typeof(ITF.FriseTextureConfig)] = 1688,
			[typeof(ITF.GFXMaterialSerializable)] = 1456,
			[typeof(ITF.GFXMaterialTexturePathSet)] = 980,
			[typeof(ITF.GFXMaterialSerializableParam)] = 24,
			[typeof(ITF.CollisionTexture)] = 32,
			[typeof(ITF.VertexAnim)] = 44,
			[typeof(ITF.FluidConfig)] = 316,
			[typeof(ITF.FluidFriseLayer)] = 348,
			[typeof(ITF.GFXPrimitiveParam)] = 112,
			[typeof(ITF.GFXMaterialShader_Template)] = 332,
			[typeof(ITF.GFX_MaterialParams)] = 104,
			[typeof(ITF.Actor_Template)] = 228,
			[typeof(ITF.TweenComponent_Template)] = 116,
			[typeof(ITF.TweenComponent_Template.InstructionSet)] = 128,
			[typeof(ITF.TweenLine_Template)] = 116,
			[typeof(ITF.TweenWait_Template)] = 16,
			[typeof(ITF.ProceduralInputData)] = 32,
			[typeof(ITF.TweenFX_Template)] = 28,
			[typeof(ITF.TweenSine_Template)] = 140,
			[typeof(ITF.CameraModifierComponent_Template)] = 372,
			[typeof(ITF.CamModifier_Template)] = 348,
			[typeof(ITF.LinkComponent_Template)] = 56,
			[typeof(ITF.ShapeComponent_Template)] = 88,
			[typeof(ITF.PhysShapeBox)] = 104,
			[typeof(ITF.RO2_PlayerForceActionComponent_Template)] = 32,
			[typeof(ITF.RO2_SpikyShellTrapComponent_Template)] = 1648,
			[typeof(ITF.FXControllerComponent_Template)] = 120,
			[typeof(ITF.FXControl)] = 128,
			[typeof(ITF.SoundComponent_Template)] = 164,
			[typeof(ITF.SoundDescriptor_Template)] = 428,
			[typeof(ITF.SoundParams)] = 92,
			[typeof(ITF.SpatializedPanning)] = 12,
			[typeof(ITF.ScreenRollOff)] = 16,
			[typeof(ITF.FxBankComponent_Template)] = 1688,
			[typeof(ITF.AABB)] = 16,
			[typeof(ITF.FxDescriptor_Template)] = 2832,
			[typeof(ITF.ITF_ParticleGenerator_Template)] = 1096,
			[typeof(ITF.ParticleGeneratorParameters)] = 752,
			[typeof(ITF.ParPhase)] = 84,
			[typeof(ITF.InputDesc)] = 12,
			[typeof(ITF.PhantomComponent_Template)] = 92,
			[typeof(ITF.PhysShapePolygon)] = 96,
			[typeof(ITF.ShapeData_Template)] = 28,
			[typeof(ITF.PhysShapeCircle)] = 8,
			[typeof(ITF.TouchScreenInputComponent_Template)] = 96,
			[typeof(ITF.RO2_EnemyBTAIComponent_Template)] = 568,
			[typeof(ITF.BehaviorTree_Template)] = 32,
			[typeof(ITF.BTNodeTemplate_Ref)] = 12,
			[typeof(ITF.BTDecider_Template)] = 56,
			[typeof(ITF.RO2_BTActionHitReflexTarget_Template)] = 76,
			[typeof(ITF.RO2_BTActionRangedAttack_Template)] = 308,
			[typeof(ITF.RO2_BTActionTickleGeneric_Template)] = 100,
			[typeof(ITF.BTDeciderHasFact_Template)] = 96,
			[typeof(ITF.BTSequence_Template)] = 32,
			[typeof(ITF.BTActionRemoveFact_Template)] = 60,
			[typeof(ITF.BTActionSetFact_Template)] = 84,
			[typeof(ITF.RO2_BTActionBubble_Template)] = 340,
			[typeof(ITF.BTActionStayIdle_Template)] = 84,
			[typeof(ITF.RO2_BTActionReceiveCrush_Template)] = 120,
			[typeof(ITF.RO2_BTActionReceiveHit_Template)] = 256,
			[typeof(ITF.RO2_EnemyBTAIComponent_Template.StiltsDataStruct)] = 60,
			[typeof(ITF.RO2_GroundAIControllerComponent_Template)] = 180,
			[typeof(ITF.StickToPolylinePhysComponent_Template)] = 176,
			[typeof(ITF.AnimatedComponent_Template)] = 2324,
			[typeof(ITF.SubAnimSet_Template)] = 436,
			[typeof(ITF.SubAnim_Template)] = 228,
			[typeof(ITF.AnimResourcePackage)] = 272,
			[typeof(ITF.TextureBankPath)] = 1268,
			[typeof(ITF.AnimPathAABB)] = 164,
			[typeof(ITF.AnimTree_Template)] = 120,
			[typeof(ITF.AnimTreeNodeSequence_Template)] = 104,
			[typeof(ITF.AnimTreeNodePlayAnim_Template)] = 148,
			[typeof(ITF.BlendTreeNodeChooseBranch_Template<ITF.AnimTreeResult>)] = 120,
			[typeof(ITF.BlendTreeNodeChooseBranch_Template<ITF.AnimTreeResult>.BlendLeaf)] = 20,
			[typeof(ITF.CriteriaDesc)] = 24,
			[typeof(ITF.BlendTreeNodeAddBranch_Template<ITF.AnimTreeResult>)] = 108,
			[typeof(ITF.BlendTreeTransition_Template<ITF.AnimTreeResult>)] = 220,
			[typeof(ITF.RO2_DRC_FXGrabComponent_Template)] = 44,
			[typeof(ITF.RO2_TouchSpringPlatformNoAnimComponent_Template)] = 32,
			[typeof(ITF.RO2_DisplayTutoIconComponent_Template)] = 42876,
			[typeof(ITF.SpawnActorPathList)] = 21408,
			[typeof(ITF.ControlLinearVolume)] = 40,
			[typeof(ITF.AdditiveLayer_Template<ITF.AnimTreeResult>)] = 88,
			[typeof(ITF.RO2_BuzzSawAIComponent_Template)] = 100,
			[typeof(ITF.PolylineComponent_Template)] = 40,
			[typeof(ITF.PolylineParameters)] = 232,
			[typeof(ITF.Doppler)] = 8,
			[typeof(ITF.TextureGraphicComponent_Template)] = 3104,
			[typeof(ITF.RO2_PaintBumperComponent_Template)] = 56,
			[typeof(ITF.BlendTreeBranchWeight)] = 36,
			[typeof(ITF.PhantomDetectorComponent_Template)] = 88,
			[typeof(ITF.RO2_TriggerBounceComponent_Template)] = 152,
			[typeof(ITF.RO2_HomeTreeGpeComponent_Template)] = 52,
			[typeof(ITF.ScreenRollOffXY)] = 32,
			[typeof(ITF.ColorComponent_Template)] = 20,
			[typeof(ITF.CheckpointComponent_Template)] = 20,
			[typeof(ITF.PrefetchTargetComponent_Template)] = 16,
			[typeof(ITF.RO2_BezierTreeComponent_Template)] = 100,
			[typeof(ITF.RO2_BezierBranchLumsChainLinkRendererComponent_Template)] = 12,
			[typeof(ITF.TweenInterpreter_Template)] = 24,
			[typeof(ITF.TweenInstructionSet_Template)] = 44,
			[typeof(ITF.RO2_BezierTreeRendererComponent_Template)] = 3296,
			[typeof(ITF.BezierCurveRenderer_Template)] = 1652,
			[typeof(ITF.AnimMeshVertexComponent_Template)] = 3216,
			[typeof(ITF.RO2_LumsChainComponent_Template)] = 568,
			[typeof(ITF.GFXParamsModifierComponent_Template)] = 20,
			[typeof(ITF.PlayerDetectorComponent_Template)] = 104,
			[typeof(ITF.TriggerComponent_Template)] = 92,
			[typeof(ITF.EventTrigger)] = 28,
			[typeof(ITF.TweenCircle_Template)] = 140,
			[typeof(ITF.RO2_SkullCoinBTAIComponent_Template)] = 84,
			[typeof(ITF.RO2_BTActionFollowActorSpring_Template)] = 168,
			[typeof(ITF.RO2_BTActionDragSpring_Template)] = 204,
			[typeof(ITF.BTActionPlayAnim_Template)] = 84,
			[typeof(ITF.RO2_MagicCurveComponent_Template)] = 1696,
			[typeof(ITF.RO2_EventRewardPickedUp)] = 52,
			[typeof(ITF.RO2_ExitRitualManagerComponent_Template)] = 724,
			[typeof(ITF.EventPlayMusic)] = 80,
			[typeof(ITF.EventBusMix)] = 68,
			[typeof(ITF.BusMix)] = 44,
			[typeof(ITF.BusDef)] = 68,
			[typeof(ITF.EventGeneric)] = 24,
			[typeof(ITF.RO2_FirePatchAIComponent_Template)] = 4680,
			[typeof(ITF.BezierCurveComponent_Template)] = 24,
			[typeof(ITF.TweenEvent_Template)] = 40,
			[typeof(ITF.AnimLightComponent_Template)] = 2160,
			[typeof(ITF.RO2_BulletLauncherComponent_Template)] = 236,
			[typeof(ITF.FactionComponent_Template)] = 20,
			[typeof(ITF.RO2_RopeComponent_Template)] = 6936,
			[typeof(ITF.ControlPitch)] = 40,
			[typeof(ITF.RO2_EventTouchScreenMandatoryZone)] = 20,
			[typeof(ITF.RO2_DRCMandatoryZoneComponent_Template)] = 24,
			[typeof(ITF.DialogComponent_Template)] = 48,
			[typeof(ITF.InstructionDialogText)] = 52,
			[typeof(ITF.InstructionDialogStop)] = 4,
			[typeof(ITF.ZRollOff)] = 12,
			[typeof(ITF.RO2_EditableShapeComponent_Template)] = 16,
			[typeof(ITF.RO2_FixedAIComponent_Template)] = 80,
			[typeof(ITF.RO2_AIChest2Behavior_Template)] = 516,
			[typeof(ITF.EventSequenceControl)] = 40,
			[typeof(ITF.AIIdleAction_Template)] = 80,
			[typeof(ITF.AIPlayAnimAction_Template)] = 60,
			[typeof(ITF.AIDestroyAction_Template)] = 64,
			[typeof(ITF.TrajectoryFollowerComponent_Template)] = 140,
			[typeof(ITF.RO2_ChallengeFireWallComponent_Template)] = 28,
			[typeof(ITF.TrajectoryNodeComponent_Template)] = 28,
			[typeof(ITF.SectoComponent_Template)] = 20,
			[typeof(ITF.MusicComponent_Template)] = 204,
			[typeof(ITF.MusicPartSet_Template)] = 20,
			[typeof(ITF.MusicPart_Template)] = 160,
			[typeof(ITF.MusicTree_Template)] = 124,
			[typeof(ITF.MusicTreeNodeComposite_Template)] = 96,
			[typeof(ITF.MusicTreeBlockSequence_Template)] = 128,
			[typeof(ITF.MusicTreeBlockRandom_Template)] = 128,
			[typeof(ITF.MusicTreeNodeSequence_Template)] = 108,
			[typeof(ITF.MusicTreeNodePlayMusic_Template)] = 100,
			[typeof(ITF.RO2_RewardEffectsPlayerComponent_Template)] = 64,
			[typeof(ITF.EventStopMusic)] = 40,
			[typeof(ITF.RO2_GameMaterial_Template)] = 384,
			[typeof(ITF.ClearColorComponent_Template)] = 16,
			[typeof(ITF.RO2_LumsPoolComponent_Template)] = 140,
			[typeof(ITF.RO2_ChallengeBonusComponent_Template)] = 56,
			[typeof(ITF.RO2_DrcLifeElementComponent_Template)] = 36,
			[typeof(ITF.RO2_BTActionRoaming_Template)] = 68,
			[typeof(ITF.RO2_BTActionAppearFromGround_Template)] = 60,
			[typeof(ITF.RO2_BTActionAppearParachute_Template)] = 300,
			[typeof(ITF.RO2_BTActionAppearBackground_Template)] = 128,
			[typeof(ITF.RO2_BTActionAppearBackgroundNinja_Template)] = 192,
			[typeof(ITF.RO2_BTActionAppearBackgroundLadders_Template)] = 168,
			[typeof(ITF.RO2_BTActionFindAttackTarget_Template)] = 68,
			[typeof(ITF.RO2_BTActionSpotTarget_Template)] = 92,
			[typeof(ITF.RO2_BTDeciderTargetInRangeToAttack_Template)] = 68,
			[typeof(ITF.RO2_BTActionUTurnToTarget_Template)] = 76,
			[typeof(ITF.RO2_BTActionHitTarget_Template)] = 184,
			[typeof(ITF.RO2_BTActionHitTarget_Template.AttackData)] = 32,
			[typeof(ITF.RO2_BTActionCharge_Template)] = 100,
			[typeof(ITF.RO2_BTActionUTurn_Template)] = 60,
			[typeof(ITF.RO2_BTActionDrag_Template)] = 136,
			[typeof(ITF.RO2_BTActionStun_Template)] = 84,
			[typeof(ITF.RO2_BTActionTorture_Template)] = 76,
			[typeof(ITF.RO2_BTActionFishing_Template)] = 72,
			[typeof(ITF.RO2_BTActionDrown_Template)] = 80,
			[typeof(ITF.RO2_BTActionSleep_Template)] = 80,
			[typeof(ITF.AnimTreeNodeRandomBranch_Template<ITF.AnimTreeResult>)] = 124,
			[typeof(ITF.RO2_SnapComponent_Template)] = 20,
			[typeof(ITF.RO2_LastPlayerTriggerComponent_Template)] = 96,
			[typeof(ITF.RO2_BTActionInstantDeath_Template)] = 204,
			[typeof(ITF.RO2_BTActionGhost_Template)] = 92,
			[typeof(ITF.RO2_EyeDoorComponent_Template)] = 176,
			[typeof(ITF.UplayActionComplete_Component_Template)] = 16,
			[typeof(ITF.RO2_BezierBranchRendererComponent_Template)] = 4,
			[typeof(ITF.EventSoundCommand)] = 32,
			[typeof(ITF.AuxReverbCommand)] = 188,
			[typeof(ITF.AuxReverbCommand.Unknown_RL_1377_sub_388210)] = 140,
			[typeof(ITF.SoundBoxInterpolatorComponent_Template)] = 52,
			[typeof(ITF.StaticMeshVertexComponent_Template)] = 1592,
			[typeof(ITF.ProceduralBoneComponent_Template)] = 16,
			[typeof(ITF.RO2_BezierBranchGrowComponent_Template)] = 4,
			[typeof(ITF.RO2_BezierBranchBoneComponent_Template)] = 4,
			[typeof(ITF.RO2_BezierBranchFxComponent_Template)] = 36,
			[typeof(ITF.KeepAliveComponent_Template)] = 28,
			[typeof(ITF.RO2_DigRegionComponent_Template)] = 4732,
			[typeof(ITF.Border)] = 92,
			[typeof(ITF.RO2_SpikyWeightComponent_Template)] = 16,
			[typeof(ITF.RO2_DiggingRegenerationComponent_Template)] = 44,
			[typeof(ITF.PointsCollisionComponent_Template)] = 36,
			[typeof(ITF.PolylineData)] = 180,
			[typeof(ITF.RO2_LumGlassBallComponent_Template)] = 32,
			[typeof(ITF.RO2_FishSwarmAIComponent_Template)] = 172,
			[typeof(ITF.RO2_MultipassBranchRendererComponent_Template)] = 4,
			[typeof(ITF.RO2_FlexMeshBranchComponent_Template)] = 4,
			[typeof(ITF.RO2_MultipassTreeRendererComponent_Template)] = 1612,
			[typeof(ITF.RO2_BezierBranchRendererPass_Template)] = 1684,
			[typeof(ITF.RO2_BezierBranchRendererSegment_Template)] = 92,
			[typeof(ITF.RO2_ShadowZonePlayerDetectorComponent_Template)] = 40,
			[typeof(ITF.RO2_ShadowZoneAIComponent_Template)] = 472,
			[typeof(ITF.RO2_ShadowZonesComponent_Template)] = 5984,
			[typeof(ITF.EventShow)] = 52,
			[typeof(ITF.TouchHoldTriggerComponent_Template)] = 36,
			[typeof(ITF.PlayAnimOnTriggerComponent_Template)] = 36,
			[typeof(ITF.RO2_TrunkComponent_Template)] = 76,
			[typeof(ITF.RO2_BTActionAppearSplinterCell_Template)] = 68,
			[typeof(ITF.RO2_BTDeciderTargetInRange_Template)] = 64,
			[typeof(ITF.RO2_BTConcurrent_Template)] = 36,
			[typeof(ITF.RO2_BTActionUpdateTimerFact_Template)] = 60,
			[typeof(ITF.BTDeciderFactEqual_Template)] = 96,
			[typeof(ITF.RO2_SeekingBulletAIComponent_Template)] = 440,
			[typeof(ITF.AIPlayActionsBehavior_Template)] = 36,
			[typeof(ITF.AISpawnAction_Template)] = 252,
			[typeof(ITF.ParticlePhysComponent_Template)] = 72,
			[typeof(ITF.ActorSpawnComponent_Template)] = 44,
			[typeof(ITF.ActorSpawnComponent_Template.SpawnData)] = 204,
			[typeof(ITF.RO2_BTActionRoamingUnderWater_Template)] = 68,
			[typeof(ITF.RO2_LaserDetectorComponent_Template)] = 16,
			[typeof(ITF.RO2_LaserGraphicComponent_Template)] = 1592,
			[typeof(ITF.RO2_StimComponent_Template)] = 132,
			[typeof(ITF.SequencePlayerComponent_Template)] = 236,
			[typeof(ITF.PlayTrajectory_evtTemplate)] = 416,
			[typeof(ITF.ObjectPath)] = 48,
			[typeof(ITF.Trajectory_Template)] = 104,
			[typeof(ITF.Spline)] = 28,
			[typeof(ITF.Spline.SplinePoint)] = 72,
			[typeof(ITF.BoolEventList)] = 24,
			[typeof(ITF.PlaySpawn_evtTemplate)] = 428,
			[typeof(ITF.PlayAnim_evtTemplate)] = 360,
			[typeof(ITF.PlayGameplay_evtTemplate)] = 184,
			[typeof(ITF.EventLockPlayers)] = 32,
			[typeof(ITF.PlaySkip_evtTemplate)] = 76,
			[typeof(ITF.PlaySound_evtTemplate)] = 380,
			[typeof(ITF.SequenceTrackInfo_Template)] = 48,
			[typeof(ITF.Actor)] = 872,
			[typeof(ITF.AnimLightComponent)] = 1408,
			[typeof(ITF.SubAnimSet)] = 632,
			[typeof(ITF.MoveChildrenComponent)] = 64,
			[typeof(ITF.SoundComponent)] = 396,
			[typeof(ITF.ObjectPath.Level)] = 24,
			[typeof(ITF.RO2_EventTriggerExitMapRitual)] = 20,
			[typeof(ITF.EventSequenceFade)] = 24,
			[typeof(ITF.EventCameraShake)] = 20,
			[typeof(ITF.AnimatedComponent)] = 1592,
			[typeof(ITF.DialogActorComponent)] = 416,
			[typeof(ITF.EventSequenceActivatePlayers)] = 20,
			[typeof(ITF.EventPause)] = 16,
			[typeof(ITF.BoolEventList.BoolEvent)] = 12,
			[typeof(ITF.EventSequenceActorSaveZ)] = 12,
			[typeof(ITF.RO2_EventLaunchCredits)] = 16,
			[typeof(ITF.RO2_MrDarkCineComponent_Template)] = 16,
			[typeof(ITF.AABBPrefetchComponent_Template)] = 20,
			[typeof(ITF.CameraDetectorComponent_Template)] = 80,
			[typeof(ITF.PlayLoop_evtTemplate)] = 68,
			[typeof(ITF.MoveChildrenComponent_Template)] = 20,
			[typeof(ITF.DialogActorComponent_Template)] = 304,
			[typeof(ITF.FixedCameraComponent_Template)] = 24,
			[typeof(ITF.AnimTreeNodeBranchTransition_Template)] = 120,
			[typeof(ITF.RO2_PhialBTComponent_Template)] = 232,
			[typeof(ITF.RO2_EventSpawnRewardHeart)] = 92,
			[typeof(ITF.GroundAIControllerComponent_Template)] = 180,
			[typeof(ITF.RO2_AMVFollowFluidComponent_Template)] = 20,
			[typeof(ITF.RO2_BigMamaComponent_Template)] = 208,
			[typeof(ITF.RO2_BossBuboComponent_Template)] = 60,
			[typeof(ITF.RO2_BuboBTAIComponent_Template)] = 84,
			[typeof(ITF.RO2_PrisonerCageComponent_Template)] = 172,
			[typeof(ITF.RO2_FriendlyBTAIComponent_Template)] = 732,
			[typeof(ITF.RO2_BTActionPrisonerRope_Template)] = 104,
			[typeof(ITF.RO2_BTActionPrisonerPole_Template)] = 88,
			[typeof(ITF.RO2_BTActionPrisonerTorture_Template)] = 120,
			[typeof(ITF.RO2_BTActionPrisonerCage_Template)] = 96,
			[typeof(ITF.RO2_BTActionPrisonerBullet_Template)] = 88,
			[typeof(ITF.RO2_BTActionDisappearBackground_Template)] = 96,
			[typeof(ITF.RO2_BTActionScoreRecap_Template)] = 100,
			[typeof(ITF.RO2_BTActionLitRune_Template)] = 68,
			[typeof(ITF.RO2_BTActionGiveReward_Template)] = 208,
			[typeof(ITF.RO2_HeartShield_Template)] = 220,
			[typeof(ITF.RO2_LeafScrewComponent_Template)] = 196,
			[typeof(ITF.RO2_BTActionEjected_Template)] = 100,
			[typeof(ITF.RO2_PrisonerCageLightComponent_Template)] = 96,
			[typeof(ITF.PlayTextBanner_evtTemplate)] = 108,
			[typeof(ITF.RO2_BezierBranchPolylineComponent_Template)] = 192,
			[typeof(ITF.RO2_AnimMeshVertexManagerComponent_Template)] = 24,
			[typeof(ITF.RO2_PrisonerPostComponent_Template)] = 100,
			[typeof(ITF.RO2_GyroControllerComponent_Template)] = 196,
			[typeof(ITF.RO2_CrankComponent_Template)] = 160,
			[typeof(ITF.TouchDetectorComponent_Template)] = 76,
			[typeof(ITF.AngleRangeTriggerComponent_Template)] = 16,
			[typeof(ITF.SpeedInputProviderComponent_Template)] = 48,
			[typeof(ITF.RO2_WaterPerturbationComponent_Template)] = 208,
			[typeof(ITF.RO2_PagePortalComponent_Template)] = 20,
			[typeof(ITF.VirtualLinkComponent_Template)] = 52,
			[typeof(ITF.EnemyDetectorComponent_Template)] = 84,
			[typeof(ITF.RO2_DoorComponent_Template)] = 24,
			[typeof(ITF.RO2_PALTeensieSpawnerComponent_Template)] = 24,
			[typeof(ITF.RelayEventComponent_Template)] = 36,
			[typeof(ITF.RelayData)] = 36,
			[typeof(ITF.RO2_EventReleasePrisoner)] = 20,
			[typeof(ITF.RO2_HangSpotComponent_Template)] = 48,
			[typeof(ITF.RO2_BTActionTickle_Template)] = 140,
			[typeof(ITF.RO2_BTActionGetWeaponBack_Template)] = 84,
			[typeof(ITF.RO2_MultipleEventTriggerComponent_Template)] = 16,
			[typeof(ITF.RO2_FishingRodComponent_Template)] = 32,
			[typeof(ITF.DeathDetectorComponent_Template)] = 24,
			[typeof(ITF.DeathTriggerComponent_Template)] = 92,
			[typeof(ITF.RO2_BackgroundDoorComponent_Template)] = 56,
			[typeof(ITF.RO2_PlayerForceActionWithDialogComponent_Template)] = 32,
			[typeof(ITF.MonologComponent_Template)] = 48,
			[typeof(ITF.MonologComponent_Template.TextData)] = 48,
			[typeof(ITF.RO2_BreakableAIComponent_Template)] = 156,
			[typeof(ITF.RO2_BreakableAIComponent_Template.DestructionStage)] = 32,
			[typeof(ITF.PlayAnimOnWeightChangeComponent_Template)] = 76,
			[typeof(ITF.RO2_SlingShotComponent_Template)] = 1852,
			[typeof(ITF.Trail_Template)] = 1520,
			[typeof(ITF.AfterFxComponent_Template)] = 420,
			[typeof(ITF.RO2_KuyALumsComponent_Template)] = 140,
			[typeof(ITF.RO2_MurphyStoneEyeComponent_Template)] = 256,
			[typeof(ITF.RO2_HeadBranchComponent_Template)] = 148,
			[typeof(ITF.RO2_RetractOnTapEyeBranchComponent_Template)] = 28,
			[typeof(ITF.TweenTeleport_Template)] = 32,
			[typeof(ITF.EventTutoSuccess)] = 12,
			[typeof(ITF.RO2_BreakablePropsManagerComponent_Template)] = 284,
			[typeof(ITF.RO2_BTActionCovertFromTarget_Template)] = 160,
			[typeof(ITF.RO2_EventWantViewDRC)] = 16,
			[typeof(ITF.RO2_EventPlayerSelectStart)] = 12,
			[typeof(ITF.PlayAnim3D_evtTemplate)] = 268,
			[typeof(ITF.Mesh3DComponent)] = 2272,
			[typeof(ITF.Animation3DSet)] = 44,
			[typeof(ITF.Light3DComponent_Template)] = 16,
			[typeof(ITF.RO2_PlayerSelectComponent_Template)] = 88,
			[typeof(ITF.WindComponent_Template)] = 36,
			[typeof(ITF.PhysForceModifierBox_Template)] = 60,
			[typeof(ITF.RO2_PALRitualManagerComponent_Template)] = 140,
			[typeof(ITF.RO2_GyroSpikyComponent_Template)] = 96,
			[typeof(ITF.DynamicLightZoneComponent_Template)] = 20,
			[typeof(ITF.Mesh3DComponent_Template)] = 2356,
			[typeof(ITF.Animation3DTree_Template)] = 120,
			[typeof(ITF.Animation3DSet_Template)] = 24,
			[typeof(ITF.TextBoxComponent_Template)] = 52,
			[typeof(ITF.FontTextArea.Style)] = 1220,
			[typeof(ITF.FontTextArea.FontSet)] = 980,
			[typeof(ITF.RO2_CameraLimiterComponent_Template)] = 84,
			[typeof(ITF.Margin)] = 16,
			[typeof(ITF.RO2_LevelPaintingSpawnerComponent_Template)] = 196,
			[typeof(ITF.RO2_HomeRoomComponent_Template)] = 16,
			[typeof(ITF.RO2_HomeNodeComponent_Template)] = 1432,
			[typeof(ITF.EventInstructionDialog)] = 76,
			[typeof(ITF.SmartLocId)] = 28,
			[typeof(ITF.RO2_PlayerCostumeComponent_Template)] = 68,
			[typeof(ITF.PlayAnimOnEventReceiveComponent_Template)] = 92,
			[typeof(ITF.EventDRCTapped)] = 36,
			[typeof(ITF.EventDRCSwiped)] = 52,
			[typeof(ITF.RO2_BezierBranchAmvComponent_Template)] = 1968,
			[typeof(ITF.RO2_BezierTreeAmvComponent_Template)] = 1592,
			[typeof(ITF.RO2_SwingRopeComponent_Template)] = 268,
			[typeof(ITF.SoftPlatformComponent_Template.BodyData)] = 40,
			[typeof(ITF.SoftPlatformComponent_Template.ConstraintData)] = 72,
			[typeof(ITF.RenderSimpleAnimComponent_Template)] = 16,
			[typeof(ITF.RO2_BubblePrizePlatformComponent_Template)] = 180,
			[typeof(ITF.RO2_TrapComponent_Template)] = 40,
			[typeof(ITF.RO2_TouchEyeTriggerComponent_Template)] = 108,
			[typeof(ITF.RO2_TouchEyeAIComponent_Template)] = 40,
			[typeof(ITF.RO2_EyeAnimationComponent_Template)] = 72,
			[typeof(ITF.SoftPlatformComponent_Template)] = 228,
			[typeof(ITF.RO2_CreatureWH_BulbComponent_Template)] = 356,
			[typeof(ITF.RO2_WaterHands_HandComponent_Template)] = 76,
			[typeof(ITF.RO2_EventHandsCaught)] = 16,
			[typeof(ITF.ProceduralSoftPlatformComponent_Template)] = 6448,
			[typeof(ITF.RO2_BTActionStilts_Template)] = 188,
			[typeof(ITF.RenderBoxComponent_Template)] = 4680,
			[typeof(ITF.TextAnimScaleComponent_Template)] = 64,
			[typeof(ITF.RO2_EventTickled)] = 12,
			[typeof(ITF.RO2_GyroDRCScreenComponent_Template)] = 40,
			[typeof(ITF.CustomCameraRangeComponent_Template)] = 16,
			[typeof(ITF.DummyComponent_Template)] = 16,
			[typeof(ITF.RO2_SequenceSwitchComponent_Template)] = 16,
			[typeof(ITF.SectoBroadcastComponent_Template)] = 16,
			[typeof(ITF.RO2_EventFreePrisoner)] = 16,
			[typeof(ITF.RO2_BreakableStackManagerAIComponent_Template)] = 3012,
			[typeof(ITF.RO2_BreakableStackElementAIComponent_Template)] = 3388,
			[typeof(ITF.RO2_InfoElementList)] = 20,
			[typeof(ITF.RO2_InfoElement)] = 68,
			[typeof(ITF.AnimationAtlas)] = 20,
			[typeof(ITF.AnimationAtlas.Key)] = 8,
			[typeof(ITF.RO2_FragmentsList)] = 20,
			[typeof(ITF.RO2_FxData)] = 48,
			[typeof(ITF.EventTweeningFullReset)] = 12,
			[typeof(ITF.RO2_EventReleaseRope)] = 16,
			[typeof(ITF.RO2_TouchSpringPlatformComponent_Template)] = 40,
			[typeof(ITF.RO2_PloufComponent_Template)] = 24,
			[typeof(ITF.RO2_EventSwitchDragonState)] = 16,
			[typeof(ITF.Animation3DTreeNodePlayAnim_Template)] = 136,
			[typeof(ITF.BlendTreeTransition_Template<ITF.Animation3DTreeResult>)] = 220,
			[typeof(ITF.Animation3DInfo_Template)] = 188,
			[typeof(ITF.RO2_SlingShotEnemyComponent_Template)] = 828,
			[typeof(ITF.RO2_EventSlingShotUnlockMurphy)] = 12,
			[typeof(ITF.PlayWait_evtTemplate)] = 132,
			[typeof(ITF.Label_evtTemplate)] = 88,
			[typeof(ITF.BoxInterpolatorComponent_Template)] = 40,
			[typeof(ITF.AFXPostProcessComponent_Template)] = 184,
			[typeof(ITF.TweenBallistic_Template)] = 136,
			[typeof(ITF.RO2_SuperPunchRitualComponent_Template)] = 24,
			[typeof(ITF.PunchStim)] = 1316,
			[typeof(ITF.RO2_ShooterActorParameterComponent_Template)] = 432,
			[typeof(ITF.Ray_VacuumData_Template)] = 12,
			[typeof(ITF.Ray_ShooterActorParameterComponent_Template.VacuumFxNames)] = 32,
			[typeof(ITF.Ray_ShooterActorParameter_StackData)] = 168,
			[typeof(ITF.RO2_EventBulletLaunch)] = 12,
			[typeof(ITF.RO2_BasculePlatformComponent_Template)] = 100,
			[typeof(ITF.BlendTreeNodeChooseBranch_Template<ITF.Animation3DTreeResult>)] = 120,
			[typeof(ITF.BlendTreeNodeAddBranch_Template<ITF.Animation3DTreeResult>)] = 108,
			[typeof(ITF.Animation3DTreeNodeSequence_Template)] = 104,
			[typeof(ITF.BlendTreeNodeChooseBranch_Template<ITF.Animation3DTreeResult>.BlendLeaf)] = 20,
			[typeof(ITF.RO2_FakeDynamicFogComponent_Template)] = 16,
			[typeof(ITF.RO2_BossJungleComponent_Template)] = 576,
			[typeof(ITF.RO2_CamModeMoverComponent_Template)] = 16,
			[typeof(ITF.RO2_MrDarkStuckComponent_Template)] = 108,
			[typeof(ITF.HitStim)] = 1284,
			[typeof(ITF.RO2_GeyserPlatformAIComponent_Template)] = 3612,
			[typeof(ITF.PhysForceModifierPolygon_Template)] = 52,
			[typeof(ITF.LinkCurveComponent_Template)] = 3100,
			[typeof(ITF.RO2_PlatformTreeComponent_Template)] = 176,
			[typeof(ITF.RO2_SoftCollision_Template)] = 24,
			[typeof(ITF.SimpleAIComponent_Template)] = 68,
			[typeof(ITF.RO2_AIBubblePrizeBehavior_Template)] = 84,
			[typeof(ITF.RO2_BubblePrize_Template)] = 40,
			[typeof(ITF.BubblePrizeContent_Template)] = 156,
			[typeof(ITF.RO2_EventSpawnRewardLum)] = 152,
			[typeof(ITF.AIBezierAction_Template)] = 80,
			[typeof(ITF.RO2_AIBlowOffAction_Template)] = 104,
			[typeof(ITF.RO2_AIFollowBezierCurveAction_Template)] = 64,
			[typeof(ITF.RO2_AIFlyIdleAction_Template)] = 68,
			[typeof(ITF.RO2_AIDeathBehavior_Template)] = 224,
			[typeof(ITF.BankIdChange)] = 88,
			[typeof(ITF.IdRedirect)] = 16,
			[typeof(ITF.RO2_BallSpawnerComponent_Template)] = 160,
			[typeof(ITF.FogBoxComponent_Template)] = 16,
			[typeof(ITF.MaskResolverComponent_Template)] = 24,
			[typeof(ITF.RO2_SpikyFlowerComponent_Template)] = 316,
			[typeof(ITF.RO2_PlatformAIComponent_Template)] = 196,
			[typeof(ITF.RO2_AspiNetworkComponent_Template)] = 60,
			[typeof(ITF.RO2_AspiNetworkComponent_Template.FxDataNet)] = 36,
			[typeof(ITF.RO2_AILumsComponent_Template)] = 20,
			[typeof(ITF.ParticleGeneratorComponent_Template)] = 4272,
			[typeof(ITF.RO2_BTActionRoamingInAir_Template)] = 68,
			[typeof(ITF.RO2_BossToadComponent_Template)] = 384,
			[typeof(ITF.RO2_BossShieldComponent_Template)] = 68,
			[typeof(ITF.RO2_EnduranceBrickComponent_Template)] = 16,
			[typeof(ITF.RO2_SwingComponent_Template)] = 84,
			[typeof(ITF.RO2_FromChallengeComponent_Template)] = 16,
			[typeof(ITF.RO2_ForceFieldComponent_Template)] = 3300,
			[typeof(ITF.Ray_ForceFieldComponent_Template.LinkEvent)] = 16,
			[typeof(ITF.HingePlatformComponent_Template)] = 100,
			[typeof(ITF.HingePlatformComponent_Template.HingeBoneData)] = 88,
			[typeof(ITF.HingePlatformComponent_Template.MovingPolylineData)] = 16,
			[typeof(ITF.RO2_BTActionJumpJanod_Template)] = 100,
			[typeof(ITF.RO2_DragSpringBTComponent_Template)] = 84,
			[typeof(ITF.RO2_BTActionDragForcing_Template)] = 156,
			[typeof(ITF.RO2_CameraZoneNeutralModifierComponent_Template)] = 32,
			[typeof(ITF.RO2_ChildLaunchComponent_Template)] = 180,
			[typeof(ITF.RO2_EventChildLaunchAll)] = 12,
			[typeof(ITF.ObjectControllerComponent_Template)] = 56,
			[typeof(ITF.RO2_TimeAttackFlagComponent_Template)] = 20,
			[typeof(ITF.RO2_SceneSettingsComponent_Template)] = 16,
			[typeof(ITF.RO2_ScoreLumAIComponent_Template)] = 152,
			[typeof(ITF.RO2_DispenserComponent_Template)] = 196,
			[typeof(ITF.RO2_BTActionStack_Template)] = 76,
			[typeof(ITF.RO2_BulletAIComponent_Template)] = 408,
			[typeof(ITF.RO2_PlayerForceActionShieldComponent_Template)] = 40,
			[typeof(ITF.RO2_CannonCheapComponent_Template)] = 140,
			[typeof(ITF.RO2_MinotaurShieldAIComponent_Template)] = 108,
			[typeof(ITF.RO2_TargetingComponent_Template)] = 72,
			[typeof(ITF.RO2_RailComponent_Template)] = 44,
			[typeof(ITF.RO2_ZeusFingerComponent_Template)] = 88,
			[typeof(ITF.RO2_LightningPatchAIComponent_Template)] = 6176,
			[typeof(ITF.FriezeContactDetectorComponent_Template)] = 32,
			[typeof(ITF.MusicTreeNodeRandom_Template)] = 104,
			[typeof(ITF.RO2_MusicalBossComponent_Template)] = 212,
			[typeof(ITF.RO2_ScoreRecapComponent_Template)] = 40,
			[typeof(ITF.RO2_EventForceConstantSpeed)] = 28,
			[typeof(ITF.RO2_MusicHeadTriggerComponent_Template)] = 24,
			[typeof(ITF.RO2_BossNodeComponent_Template)] = 16,
			[typeof(ITF.RO2_ChallengeFireComponent_Template)] = 24,
			[typeof(ITF.RO2_DisplayTutoIconKillerComponent_Template)] = 16,
			[typeof(ITF.ToggleAnimOnEventComponent_Template)] = 32,
			[typeof(ITF.HingePlatformComponent_Template.PlatformData)] = 72,
			[typeof(ITF.EventStickOnPolyline)] = 48,
			[typeof(ITF.RO2_ScaleTunnelComponent_Template)] = 124,
			[typeof(ITF.SubAnchorComponent_Template)] = 36,
			[typeof(ITF.SubAnchor_Template)] = 52,
			[typeof(ITF.RO2_AIUtensilTrapBehavior_Template)] = 44,
			[typeof(ITF.RO2_JalapenoKingAiComponent_Template)] = 108,
			[typeof(ITF.GenericAIComponent_Template)] = 56,
			[typeof(ITF.RO2_AIHarissaToggleBehavior_Template)] = 84,
			[typeof(ITF.RO2_AIPerformHitPolylinePunchAction_Template)] = 96,
			[typeof(ITF.RO2_EventPowerUp)] = 40,
			[typeof(ITF.RO2_ShooterCheckPointComponent_Template)] = 72,
			[typeof(ITF.PlayerSpawnPos)] = 16,
			[typeof(ITF.RO2_ShooterCameraComponent_Template)] = 36,
			[typeof(ITF.RO2_ShooterCameraModifierComponent_Template)] = 16,
			[typeof(ITF.BlendTreeNodeChooseBranch_Template<ITF.MusicTreeResult>)] = 120,
			[typeof(ITF.RO2_BTActionThrowStone_Template)] = 268,
			[typeof(ITF.RO2_NautilusAIComponent_Template)] = 144,
			[typeof(ITF.RO2_BTActionHelmet_Template)] = 80,
			[typeof(ITF.RO2_BTActionChaseTarget_Template)] = 76,
			[typeof(ITF.RO2_BTActionLaunch_Template)] = 76,
			[typeof(ITF.RO2_ElevatorAIComponent_Template)] = 60,
			[typeof(ITF.RO2_ElevatorWheelAIComponent_Template)] = 16,
			[typeof(ITF.RO2_BTActionBlowing_Template)] = 100,
			[typeof(ITF.RO2_BTActionMusicScore_Template)] = 72,
			[typeof(ITF.RO2_MusicScoreSnapComponent_Template)] = 76,
			[typeof(ITF.RO2_StringWaveGeneratorComponent_Template)] = 140,
			[typeof(ITF.RO2_MusicScoreAIBranchComponent_Template)] = 28,
			[typeof(ITF.RO2_AINotePiafBehavior_Template)] = 96,
			[typeof(ITF.AIFallNoPhysAction_Template)] = 72,
			[typeof(ITF.AIBallisticsApexAction_Template)] = 68,
			[typeof(ITF.RO2_BubblePrizeBumperComponent_Template)] = 180,
			[typeof(ITF.RO2_StringWaveFaderComponent_Template)] = 76,
			[typeof(ITF.AtlasAnimationComponent_Template)] = 3060,
			[typeof(ITF.RO2_PushButtonComponent_Template)] = 60,
			[typeof(ITF.RO2_DjembeComponent_Template)] = 36,
		};

		public static readonly Dictionary<Type, uint> RetailSizes = new Dictionary<Type, uint>() {
			// Main
			[typeof(StringID)] = 4,
			[typeof(Path)] = 136, // 132 + StringID
			[typeof(LocalizedPath)] = 140, // Path + LocId
			[typeof(LocalisationId)] = 4,
			[typeof(ColorInteger)] = 4,
			[typeof(string)] = 20,
			[typeof(Vec2d)] = 8,
			[typeof(Vec3d)] = 12,
			[typeof(Vec4d)] = 16,
			[typeof(Color)] = 16,
			[typeof(Angle)] = 4,
			[typeof(AngleAmount)] = 4,
			[typeof(ObjectId)] = 8,
			[typeof(ObjectRef)] = 4,
			[typeof(PathRef)] = 4,
			[typeof(Platform)] = 4,
			[typeof(SoundGUID)] = 4,
			[typeof(Volume)] = 4,

			// From IDA
		};

		public static readonly Dictionary<Type, uint> RetailAlign = new Dictionary<Type, uint>() {
			// ObjectFactory
			[typeof(ITF.Scene)] = 16,


			// Not ObjectFactory
			[typeof(ITF.Trajectory)] = 16,
			[typeof(ITF.Message)] = 8,
			[typeof(ITF.UIScrollbar_Template.Style)] = 1,
			[typeof(ITF.Frise.MeshStaticData)] = 16,
			[typeof(ITF.Frise.MeshAnimData)] = 16,
			[typeof(ITF.Frise.MeshOverlayData)] = 16,
		};
		#endregion

		public const bool UseEditorSizes = true;

		public bool TryGetSize(Type type, out uint size) {
			uint? sizeOf = null;
			size = 0;
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				sizeOf = 4;
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: sizeOf = 4; break;
					case TypeCode.Byte: sizeOf = 1; break;
					case TypeCode.Char: sizeOf = 1; break;
					case TypeCode.String: sizeOf = 20; break; // sizeof(String8) // IN RL
					case TypeCode.Double: sizeOf = 8; break;
					case TypeCode.UInt16: sizeOf = 2; break;
					case TypeCode.UInt64: sizeOf = 8; break;
					case TypeCode.Int16: sizeOf = 2; break;
					case TypeCode.Int64: sizeOf = 8; break;
				}
			} else if (typeof(IObjectContainer).IsAssignableFrom(type)) {
				sizeOf = 4;
				if (type.IsGenericType) {
					var baseType = type.GetGenericTypeDefinition();
					// Could probably be written better
					if (baseType == typeof(CList<>) || baseType == typeof(CArray<>)
						|| baseType == typeof(CListO<>) || baseType == typeof(CListP<>) || baseType == typeof(CRefList<>)
						|| baseType == typeof(CArrayO<>) || baseType == typeof(CArrayP<>)) {
						sizeOf = 20;
					}
				}
			}
			
			if(!sizeOf.HasValue) {
				if (UseEditorSizes && EditorSizes.ContainsKey(type)) {
					sizeOf = EditorSizes[type];
				}
				if (!sizeOf.HasValue && RetailSizes.ContainsKey(type)) {
					sizeOf = RetailSizes[type];
				}
			}

			if (sizeOf.HasValue) {
				size = sizeOf.Value;
				return true;
			}
			return false;
		}


		public bool TryGetAlign(Type type, out uint align) {
			if (RetailAlign.ContainsKey(type)) {
				align = RetailAlign[type];
				return true;
			}
			align = 0;
			return false;
		}
	}
}