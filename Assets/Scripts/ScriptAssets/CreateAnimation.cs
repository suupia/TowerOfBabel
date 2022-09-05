using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public static class CreateAnimation
{

    [MenuItem("CONTEXT/SpriteRenderer/CreateAnimation")]
    private static void CreateAnimattion(MenuCommand menuCommand)
    {
        const int pixelsPerUnit = 32;
        const int frameRate = 10; //AnimationClipのSamplesに対応するもの
        const string destinationPathForAnimationClips = "Assets/Animations/Units/AnimationClips";
        const string destinationPathForAnimatorOverrideController = "Assets/Animations/Units/AnimatorControllers";


        Debug.Log("CreateAnimationを実行します");

        //スプライトを取得
        Sprite sprite = (menuCommand.context as SpriteRenderer).sprite;

        //名前を設定
        string name = sprite.name;
        //name = name.Substring(8); //先頭から8文字を削除（日付の分）
        name = name.Substring(0, name.Length - 2); //末尾のハイフンと数字を削除（_0）
        //Debug.Log($"name:{name}");

        //列の数を取得
        int columnNum = sprite.texture.width / pixelsPerUnit;

        //パスを取得
        string path = AssetDatabase.GetAssetPath(sprite);

        //スライスされたスプライトを取得
        Sprite[] slicedSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

        //アニメーションオーバーライドコントローラを作成
        var overrideController = new AnimatorOverrideController();
        overrideController.name = name;
        overrideController.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{destinationPathForAnimatorOverrideController}/Slime.controller");


        //前、横、後ろ、斜め前、斜め後ろの移動と被ダメのアニメーションクリップを作る
        string[] suffixes = new string[] { "Front", "Side", "Back", "DiagonalFront", "DiagonalBack" };
        for (int rowIndex = 0; rowIndex < suffixes.Length; rowIndex++)
        {
            CreateMovementAnimationClip(rowIndex, suffixes[rowIndex]);
            CreateDamagedAnimationClip(rowIndex, suffixes[rowIndex]);
        }

        //最後にアニメーションオーバーライドコントローラを保存
        AssetDatabase.CreateAsset(
                overrideController,
                AssetDatabase.GenerateUniqueAssetPath($"{destinationPathForAnimatorOverrideController}/{overrideController.name}.anim")
            );
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //##########################################################

        //ローカル関数
        void CreateMovementAnimationClip(int rowIndex, string suffix)
        {
            var clip = new AnimationClip();
            var binding = new EditorCurveBinding();

            binding.path = "";
            binding.type = typeof(SpriteRenderer);
            binding.propertyName = "m_Sprite";

            //キーフレーム設定
            var keyFrames = new List<ObjectReferenceKeyframe>();
            for (int i = 0; i < columnNum - 1; i++) //最後の１枚は被ダメ用
            {
                keyFrames.Add(new ObjectReferenceKeyframe()
                {
                    time = (float)i / frameRate,
                    value = slicedSprites[i + columnNum * rowIndex]
                });
                Debug.Log($"slicedSprites[{i + columnNum * rowIndex}]:{slicedSprites[i + columnNum * rowIndex]}");
            }

            //名前、フレームレート、ループ設定
            clip.name = name + suffix;
            clip.frameRate = frameRate;
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            //登録
            AnimationUtility.SetObjectReferenceCurve(
                clip,
                binding,
                keyFrames.ToArray());

            //アニメーションオーバーライドコントローラに登録
            overrideController[$"Slime{suffix}"] = clip;

            //アニメーションクリップファイルを保存
            AssetDatabase.CreateAsset(
                    clip,
                    AssetDatabase.GenerateUniqueAssetPath($"{destinationPathForAnimationClips}/{clip.name}.anim")
                );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //Debug.Log("CreateMovementAnimationClipを終了");
        }

        void CreateDamagedAnimationClip(int rowIndex, string suffix)
        {
            var clip = new AnimationClip();
            var binding = new EditorCurveBinding();

            binding.path = "";
            binding.type = typeof(SpriteRenderer);
            binding.propertyName = "m_Sprite";

            //キーフレーム設定
            var keyFrames = new List<ObjectReferenceKeyframe>();

            keyFrames.Add(new ObjectReferenceKeyframe()
            {
                time = (float)(columnNum - 1) / frameRate, //iをcolumnNum-1に置き換えればよい
                value = slicedSprites[columnNum - 1 + columnNum * rowIndex]
            });
            //Debug.Log($"slicedSprites[{columnNum - 1 + columnNum * rowIndex}]:{slicedSprites[columnNum - 1 + columnNum * rowIndex]}");



            //名前、フレームレート、ループ設定
            clip.name = name + "Damaged" + suffix;
            clip.frameRate = frameRate;
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            //登録
            AnimationUtility.SetObjectReferenceCurve(
                clip,
                binding,
                keyFrames.ToArray());

            //アニメーションオーバーライドコントローラに登録
            overrideController[$"SlimeDamaged{suffix}"] = clip;

            //アニメーションクリップファイルを保存
            AssetDatabase.CreateAsset(
                    clip,
                    AssetDatabase.GenerateUniqueAssetPath($"{destinationPathForAnimationClips}/{clip.name}.anim")
                );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //Debug.Log("CreateDamagedAnimationClipを終了");
        }
    }


}

