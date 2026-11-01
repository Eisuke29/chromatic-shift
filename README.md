# CHROMATIC SHIFT

色を切り替えて箱を消し、道を作るUnity製3Dパズル。

[作品を見る](https://eisuke29.github.io/portfolio/work3.html) · [ポートフォリオ](https://eisuke29.github.io/portfolio/)

![CHROMATIC SHIFTの画面](media/cover.jpg)

## ゲーム

赤・青・黄を切り替えると、その色の箱が消えます。色と箱の配置を考え、道を作ってゴールを目指します。2026年6月23日発表の個人制作。

## ローカルで動かす

このリポジトリのフォルダで次を実行し、http://localhost:8000/ を開きます。

```sh
python -m http.server 8000
```

PCのWebGL対応ブラウザを使用します。WASDで移動、Spaceでジャンプ。色の切り替えは画面内のカラーホイールから行います。

## ソースの範囲

- `web/play/chromatic/`：公開用Unity WebGL完成ビルド
- `source/Unity/Assets/Scripts/`：自作の色制御・箱操作・ゴール判定・音・UIのC#コード
- `source/Unity/Assets/Editor/WebGLBuild.cs`：WebGL書き出し用スクリプト

Unity 6.4（6000.4.11f1）で制作しました。Unityプロジェクト全体ではなく自作コードの抜粋です。Starter Assets、TextMesh Pro、音源、シーン等の素材はソースとして再配布していません。ソースだけを開いて再ビルドはできません。


## 制作者・権利

木下瑛介 / Eisuke Kinoshita（[Eisuke29](https://github.com/Eisuke29)）。共同制作・外部素材については上記の説明を参照してください。自作のコード・映像・デザインについて、別途記載のない限り無断での再配布は許諾していません。外部ライブラリ・モデル・フォント等にはそれぞれのライセンスが適用されます。
