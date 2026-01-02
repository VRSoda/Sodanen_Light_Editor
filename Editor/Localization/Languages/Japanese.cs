using System.Collections.Generic;

namespace Brightness.Localization
{
    public static class Japanese
    {
        public static readonly Dictionary<string, string> Strings = new()
        {
            // ===== Menu & Window =====
            { "menu.title", "明るさ調整エディター" },
            { "window.title", "Sodanen エディター" },

            // ===== Avatar Section =====
            { "avatar.title", "アバター" },
            { "avatar.target", "対象" },
            { "avatar.select", "アバターを選択してください" },
            { "avatar.select_dropdown", "-- アバター選択 --" },
            { "avatar.material_count", "lilToon マテリアル: {0}個" },
            { "avatar.no_descriptor", "VRCAvatarDescriptor なし" },

            // ===== Feature Section =====
            { "feature.title", "機能選択" },
            { "feature.light", "ライト" },
            { "feature.shadow", "シャドウ" },
            { "feature.minlight", "Min Light" },
            { "feature.maxlight", "Max Light" },
            { "feature.backlight", "Back Light" },
            { "feature.shadow_main", "Shadow" },
            { "feature.shadow_x", "Shadow X Angle" },
            { "feature.shadow_y", "Shadow Y Angle" },
            { "feature.minlight.tooltip", "暗い環境での最小明るさ制限" },
            { "feature.maxlight.tooltip", "明るい環境での最大明るさ制限" },
            { "feature.backlight.tooltip", "後ろからの光の強度" },
            { "feature.shadow.tooltip", "影の方向調整" },
            { "feature.shadow_x.tooltip", "X軸の影の方向" },
            { "feature.shadow_y.tooltip", "Y軸の影の方向" },

            // ===== Material Settings =====
            { "material.title", "マテリアル設定" },
            { "material.window_title", "マテリアルエディター" },
            { "material.unify_toggle", "プロパティ統一 / 個別調整" },
            { "material.select_all", "全て" },
            { "material.deselect_all", "解除" },

            // ===== Unify Section =====
            { "unify.title", "マテリアルプロパティ統一" },
            { "unify.check_before", "適用前にマテリアルの初期値が同一か確認" },
            { "unify.source_material", "マテリアルから取得" },
            { "unify.shadow", "シャドウ" },
            { "unify.mask_strength", "マスクと強度" },
            { "unify.strength", "強度" },
            { "unify.shadow_1", "影色 1" },
            { "unify.shadow_2", "影色 2" },
            { "unify.shadow_3", "影色 3" },
            { "unify.color", "色" },
            { "unify.range", "範囲" },
            { "unify.blur", "ぼかし" },
            { "unify.normal_strength", "ノーマルマップ強度" },
            { "unify.receive_shadow", "影を受ける" },
            { "unify.alpha", "透明度" },
            { "unify.border_color", "境界色" },
            { "unify.border_range", "境界幅" },
            { "unify.contrast", "コントラスト" },
            { "unify.env_strength", "影色への環境光の影響" },
            { "unify.blur_mask", "ぼかしエフェクトマスク" },
            { "unify.compare", "変更前の数値を比較" },
            { "unify.apply_material", "マテリアル適用" },

            // ===== Non-Destructive Mode =====
            { "nondestructive.title", "非破壊モード" },
            { "nondestructive.description", "アバターとマテリアルを複製します。元は変更されません。" },
            { "nondestructive.duplicate_only", "複製のみ" },
            { "nondestructive.duplicate_unify", "複製 + 統一" },
            { "nondestructive.delete_all", "複製削除" },

            // ===== Custom Material =====
            { "custom.title", "個別マテリアル影調整 ({0}個)" },
            { "custom.add_help", "+ボタンでマテリアルを追加してください" },
            { "custom.exclude_notice", "これらのマテリアルは「マテリアルプロパティ統一」から除外されます" },
            { "custom.select_material", "(マテリアル選択)" },
            { "custom.material", "マテリアル" },
            { "custom.current_value", "現在値" },
            { "custom.shadow_1", "影 1" },
            { "custom.shadow_2", "影 2" },
            { "custom.shadow_3", "影 3" },

            // ===== Preset Section =====
            { "preset.title", "プリセット" },
            { "preset.management", "プリセット管理" },
            { "preset.refresh", "更新" },
            { "preset.empty", "保存されたプリセットがありません。" },
            { "preset.save_current", "現在の設定を保存" },
            { "preset.new_name", "新規プリセット {0}" },
            { "preset.apply", "適用" },
            { "preset.rename", "名前変更" },
            { "preset.delete", "削除" },
            { "preset.export", "エクスポート" },
            { "preset.import", "インポート" },
            { "preset.default_name", "デフォルト設定" },
            { "preset.default_desc", "lilToon デフォルト値" },

            // ===== Dialogs =====
            { "dialog.result", "結果" },
            { "dialog.complete", "完了" },
            { "dialog.warning", "警告" },
            { "dialog.error", "エラー" },
            { "dialog.confirm", "確認" },
            { "dialog.apply", "適用" },
            { "dialog.cancel", "キャンセル" },
            { "dialog.close", "閉じる" },
            { "dialog.select", "選択" },
            { "dialog.save", "保存" },
            { "dialog.change", "変更" },
            { "dialog.overwrite", "上書き" },
            { "dialog.duplicate", "複製" },

            // ===== Dialog Messages =====
            { "dialog.all_materials_match", "すべてのマテリアルの数値が同一です。" },
            { "dialog.different_materials", "数値が異なるマテリアル: {0}個" },
            { "dialog.unify_title", "マテリアル統一" },
            { "dialog.unify_message", "選択したプロパティ値をすべてのlilToonマテリアルに適用します。\nこの操作は元に戻せません。" },
            { "dialog.unify_complete", "マテリアルプロパティが統一されました。" },
            { "dialog.duplicate_title", "アバター複製" },
            { "dialog.duplicate_message", "アバターとすべてのlilToonマテリアルを複製します。\n\n• 複製されたアバターを作成\n• マテリアルを新しいフォルダに保存\n• 複製アバターをX軸-0.5移動" },
            { "dialog.duplicate_unify_title", "アバター複製とプロパティ統一" },
            { "dialog.duplicate_unify_message", "アバターとマテリアルを複製し、プロパティを適用します。\n\n• 複製されたアバターを作成\n• マテリアルプロパティを統一\n• 複製アバターをX軸-0.5移動" },
            { "dialog.duplicate_unify_button", "複製と統一" },
            { "dialog.delete", "削除" },
            { "dialog.delete_all_title", "複製アバターとマテリアル削除" },
            { "dialog.delete_all_message", "現在選択中の複製アバターとマテリアルフォルダを削除します。\n\nこの操作は元に戻せません。" },
            { "dialog.delete_all_complete", "削除完了！\n\nアバター: {0}\nマテリアルフォルダ: {1}" },
            { "dialog.delete_avatar_complete", "アバター '{0}' が削除されました。" },
            { "dialog.not_duplicated_avatar", "複製されたアバター(_Copy)のみ削除できます。" },
            { "dialog.no_liltoon", "lilToonシェーダーを使用しているオブジェクトが見つかりません。" },
            { "dialog.apply_complete", "明るさ調整機能が適用されました！\n\n適用された機能（オブジェクト数）:\n{0}" },

            // ===== Preset Dialogs =====
            { "preset.save_title", "新規プリセット保存" },
            { "preset.name_label", "名前" },
            { "preset.desc_label", "説明" },
            { "preset.name_required", "プリセット名を入力してください。" },
            { "preset.rename_title", "プリセット名変更" },
            { "preset.new_name_label", "新しい名前" },
            { "preset.overwrite_title", "プリセット上書き" },
            { "preset.overwrite_message", "プリセット '{0}' は既に存在します。\n上書きしますか？" },
            { "preset.save_complete_title", "保存完了" },
            { "preset.save_complete_message", "プリセット '{0}' が保存されました。" },
            { "preset.applied_log", "[Preset] '{0}' プリセット適用 (個別マテリアル: {1}個)" },
            { "preset.delete_title", "プリセット削除" },
            { "preset.delete_message", "プリセット '{0}' を削除しますか？\nこの操作は元に戻せません。" },
            { "preset.export_select", "まずエクスポートするプリセットを選択してください。" },
            { "preset.export_title", "プリセットエクスポート" },
            { "preset.export_complete_title", "エクスポート完了" },
            { "preset.export_complete_message", "プリセットが保存されました:\n{0}" },
            { "preset.import_title", "プリセットインポート" },
            { "preset.import_complete_title", "インポート完了" },
            { "preset.import_complete_message", "プリセット '{0}' がインポートされました。" },

            // ===== Buttons =====
            { "button.apply", "適用する" },
            { "button.select_avatar", "アバターを選択してください" },
            { "button.select_feature", "少なくとも1つの機能を選択してください" },

            // ===== Cloth Editor =====
            { "cloth.title", "トグルグループ" },
            { "cloth.selected_count", "登録アイテム: {0}個" },
            { "cloth.no_items", "グループを追加してください" },
            { "cloth.settings", "設定" },
            { "cloth.saved_state", "状態を保存" },
            { "cloth.apply", "Toggle作成" },
            { "cloth.select_items", "アイテムを追加してください" },
            { "cloth.apply_complete", "{0}個の衣装トグルが作成されました！" },
            { "cloth.drop_here", "ここにドラッグ＆ドロップ" },
            { "cloth.default_index", "デフォルト" },
            { "cloth.target_menu", "対象メニュー" },
            { "cloth.add_all_off", "All OFF追加" },
            { "cloth.toggle_name", "パラメータ" },
            { "cloth.save_folder", "保存フォルダ" },
            { "cloth.default_folder", "(デフォルト位置)" },
            { "cloth.select_folder", "保存フォルダ選択" },
            { "cloth.folder_error", "Assetsフォルダ内のみ選択できます。" },
            { "cloth.new_menu", "新規メニュー作成" },

            // ===== Update =====
            { "update.available", "→ v{0} アップデート可能 (VCC確認)" },

            // ===== Language =====
            { "language.title", "言語" },
        };
    }
}
