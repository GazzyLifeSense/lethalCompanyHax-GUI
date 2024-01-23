using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    public static List<string> PromptEnemiesToTarget(PlayerControllerB player) {
        List<string> enemyNames = [];

        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            return enemyNames;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            return enemyNames;
        }

        _ = Reflector.Target(roundManager).InvokeInternalMethod("RefreshEnemiesList");

        roundManager.SpawnedEnemies.ForEach((enemy) => {
            enemy.isEnemyDead = false;
            enemy.enemyHP = enemy.enemyHP <= 0 ? 5 : enemy.enemyHP;
            enemy.targetPlayer = player;
            enemyNames.Add(enemy.enemyType.enemyName);
            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

            if (enemy is CrawlerAI thumper) {
                thumper.BeginChasingPlayerServerRpc((int)player.playerClientId);
            }

            else if (enemy is MouthDogAI eyelessDog) {
                eyelessDog.ReactToOtherDogHowl(player.transform.position);
            }

            else if (enemy is BaboonBirdAI baboonHawk) {
                Threat threat = new() {
                    threatScript = player,
                    lastSeenPosition = player.transform.position,
                    threatLevel = int.MaxValue,
                    type = ThreatType.Player,
                    focusLevel = int.MaxValue,
                    timeLastSeen = Time.time,
                    distanceToThreat = 0.0f,
                    distanceMovedTowardsBaboon = float.MaxValue,
                    interestLevel = int.MaxValue,
                    hasAttacked = true
                };

                _ = Reflector.Target(baboonHawk).InvokeInternalMethod("ReactToThreat", threat);
            }

            else if (enemy is ForestGiantAI giant) {
                giant.chasingPlayer = player;
                giant.timeSpentStaring = 10;
                giant.SwitchToBehaviourState(1);
                _ = Reflector.Target(giant).SetInternalField("lostPlayerInChase", false);
            }

            else if (enemy is SandWormAI or MaskedPlayerEnemy or SpringManAI) {
                enemy.SwitchToBehaviourState(1);
            }

            else if (enemy is CentipedeAI or PufferAI or JesterAI) {
                enemy.SwitchToBehaviourState(2);
            }

            else if (enemy is FlowermanAI bracken) {
                bracken.SwitchToBehaviourState(2);
                bracken.EnterAngerModeServerRpc(20);
            }

            else if (enemy is SandSpiderAI spider) {
                spider.meshContainer.position = player.transform.position;
                spider.SwitchToBehaviourState(2);
                spider.SyncMeshContainerPositionToClients();
                _ = Reflector.Target(spider)
                             .SetInternalField("onWall", false)?
                             .SetInternalField("watchFromDistance", false);
            }

            else if (enemy is HoarderBugAI hoardingBug) {
                hoardingBug.angryAtPlayer = player;
                hoardingBug.angryTimer = 1000;
                hoardingBug.SwitchToBehaviourState(2);
                _ = Reflector.Target(hoardingBug)
                             .SetInternalField("lostPlayerInChase", false)?
                             .InvokeInternalMethod("SyncNestPositionServerRpc", player.transform.position);
            }

            else if (enemy is RedLocustBees bees) {
                bees.SwitchToBehaviourState(2);
                bees.hive.isHeld = true;
            }

            else if (enemy is NutcrackerEnemyAI nutcracker) {
                nutcracker.SwitchToBehaviourState(2);
                nutcracker.SeeMovingThreatServerRpc((int)player.playerClientId);
                _ = Reflector.Target(nutcracker)
                             .SetInternalField("lastSeenPlayerPos", player.transform.position)?
                             .SetInternalField("timeSinceSeeingTarget", 0);
            }
        });

        return enemyNames;
    }
}
