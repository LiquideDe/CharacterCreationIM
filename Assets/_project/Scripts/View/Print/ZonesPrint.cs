using CharacterCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public enum BodyZone { Head, LeftArm, RightArm, Body, LeftLeg, RightLeg }
    public class ZonesPrint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textHead;
        [SerializeField] private TextMeshProUGUI _textLeftHand;
        [SerializeField] private TextMeshProUGUI _textRightHand;
        [SerializeField] private TextMeshProUGUI _textLeftLeg;
        [SerializeField] private TextMeshProUGUI _textRightleg;
        [SerializeField] private TextMeshProUGUI _textBody;

        private Dictionary<BodyZone, int>  armorByZone = new Dictionary<BodyZone, int>
        {
            { BodyZone.Head, 0 },
            { BodyZone.LeftArm, 0 },
            { BodyZone.RightArm, 0 },
            { BodyZone.Body, 0 },
            { BodyZone.LeftLeg, 0 },
            { BodyZone.RightLeg, 0 },
        };
        
        private Dictionary<string, List<BodyZone>> _zoneMap =
            new Dictionary<string, List<BodyZone>>(StringComparer.OrdinalIgnoreCase)
            {
            // Голова
            ["голова"] = new() { BodyZone.Head },
            ["head"] = new() { BodyZone.Head },

            // Руки
            ["леваярука"] = new() { BodyZone.LeftArm },
            ["левая рука"] = new() { BodyZone.LeftArm },
            ["левая_рука"] = new() { BodyZone.LeftArm },
            ["левая-рука"] = new() { BodyZone.LeftArm },
            ["leftarm"] = new() { BodyZone.LeftArm },

            ["праваярука"] = new() { BodyZone.RightArm },
            ["правая рука"] = new() { BodyZone.RightArm },
            ["правая_рука"] = new() { BodyZone.RightArm },
            ["правая-рука"] = new() { BodyZone.RightArm },
            ["rightarm"] = new() { BodyZone.RightArm },

            // Тело
            ["тело"] = new() { BodyZone.Body },
            ["торс"] = new() { BodyZone.Body },
            ["грудь"] = new() { BodyZone.Body },
            ["body"] = new() { BodyZone.Body },
            ["torso"] = new() { BodyZone.Body },
            ["chest"] = new() { BodyZone.Body },

            // Ноги
            ["леваянога"] = new() { BodyZone.LeftLeg },
            ["левая нога"] = new() { BodyZone.LeftLeg },
            ["левая_нога"] = new() { BodyZone.LeftLeg },
            ["левая-нога"] = new() { BodyZone.LeftLeg },
            ["leftleg"] = new() { BodyZone.LeftLeg },

            ["праваянога"] = new() { BodyZone.RightLeg },
            ["правая нога"] = new() { BodyZone.RightLeg },
            ["правая_нога"] = new() { BodyZone.RightLeg },
            ["правая-нога"] = new() { BodyZone.RightLeg },
            ["rightleg"] = new() { BodyZone.RightLeg },

            // Групповые (на всякий)
            ["руки"] = new() { BodyZone.LeftArm, BodyZone.RightArm },
            ["ноги"] = new() { BodyZone.LeftLeg, BodyZone.RightLeg },
            ["все"] = new() { BodyZone.Head, BodyZone.LeftArm, BodyZone.RightArm, BodyZone.Body, BodyZone.LeftLeg, BodyZone.RightLeg }
        };

        public void SetArmorPoints(List<ArmorData> armors, List<AugmeticData> augmetics)
        {
            if (armors != null)
            {
                foreach (var a in armors)
                {
                    if (a?.protectionZones == null) continue;

                    foreach (var token in a.protectionZones)
                    {
                        foreach (var z in MapZones(token))
                        {
                            // броня не складывается — выбираем максимум
                            armorByZone[z] = Math.Max(armorByZone[z], a.armorPoints);
                        }
                    }
                }
            }

            if (augmetics != null)
            {
                foreach (var aug in augmetics)
                {
                    if (aug == null || aug.armor <= 0) continue;

                    var zones = MapZones(aug.place);
                    if (zones.Count == 0)
                    {
                        Debug.LogAssertion($"[Armor] Неизвестная зона у аугметики '{aug.name}': '{aug.place}'");
                        continue;
                    }

                    foreach (var z in zones)
                        armorByZone[z] += aug.armor;
                }
            }

            _textHead.text = armorByZone[BodyZone.Head].ToString();
            _textLeftHand.text = armorByZone[BodyZone.LeftArm].ToString();
            _textLeftLeg.text = armorByZone[BodyZone.LeftLeg].ToString();
            _textRightHand.text = armorByZone[BodyZone.RightArm].ToString();
            _textRightleg.text = armorByZone[BodyZone.RightLeg].ToString();
            _textBody.text = armorByZone[BodyZone.Body].ToString();
        }

        private List<BodyZone> MapZones(string raw)
        {
            var key = Normalize(raw);
            if (string.IsNullOrEmpty(key)) return new List<BodyZone>();
            if (_zoneMap.TryGetValue(key, out var list)) return list;

            // попытка без пробелов/дефисов
            key = key.Replace(" ", "").Replace("-", "").Replace("_", "");
            if (_zoneMap.TryGetValue(key, out list)) return list;

            // неизвестная зона
            return new List<BodyZone>();
        }

        private string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim().ToLowerInvariant().Replace('ё', 'е');
            return s;
        }
    }
}

