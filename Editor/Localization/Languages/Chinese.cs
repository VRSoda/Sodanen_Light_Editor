using System.Collections.Generic;

namespace Brightness.Localization
{
    public static class Chinese
    {
        public static readonly Dictionary<string, string> Strings = new()
        {
            // ===== Menu & Window =====
            { "menu.title", "亮度调节编辑器" },
            { "window.title", "Sodanen 编辑器" },

            // ===== Avatar Section =====
            { "avatar.title", "角色" },
            { "avatar.target", "目标" },
            { "avatar.select", "请选择角色" },
            { "avatar.material_count", "lilToon 材质: {0}个" },
            { "avatar.no_descriptor", "无 VRCAvatarDescriptor" },

            // ===== Feature Section =====
            { "feature.title", "功能选择" },
            { "feature.light", "光照" },
            { "feature.shadow", "阴影" },
            { "feature.minlight", "最小亮度" },
            { "feature.maxlight", "最大亮度" },
            { "feature.backlight", "背光" },
            { "feature.shadow_main", "阴影" },
            { "feature.shadow_x", "阴影 X 角度" },
            { "feature.shadow_y", "阴影 Y 角度" },
            { "feature.minlight.tooltip", "暗环境下的最小亮度限制" },
            { "feature.maxlight.tooltip", "亮环境下的最大亮度限制" },
            { "feature.backlight.tooltip", "来自背后的光照强度" },
            { "feature.shadow.tooltip", "阴影方向控制" },
            { "feature.shadow_x.tooltip", "X轴阴影方向" },
            { "feature.shadow_y.tooltip", "Y轴阴影方向" },

            // ===== Material Settings =====
            { "material.title", "材质设置" },
            { "material.unify_toggle", "属性统一 / 单独调节" },
            { "material.select_all", "全选" },
            { "material.deselect_all", "取消" },

            // ===== Unify Section =====
            { "unify.title", "统一材质属性" },
            { "unify.check_before", "应用前确认材质初始值是否相同" },
            { "unify.source_material", "从材质导入" },
            { "unify.shadow", "阴影" },
            { "unify.mask_strength", "遮罩和强度" },
            { "unify.strength", "强度" },
            { "unify.shadow_1", "阴影颜色 1" },
            { "unify.shadow_2", "阴影颜色 2" },
            { "unify.shadow_3", "阴影颜色 3" },
            { "unify.color", "颜色" },
            { "unify.range", "范围" },
            { "unify.blur", "模糊" },
            { "unify.normal_strength", "法线贴图强度" },
            { "unify.receive_shadow", "接收阴影" },
            { "unify.alpha", "透明度" },
            { "unify.border_color", "边界颜色" },
            { "unify.border_range", "边界宽度" },
            { "unify.contrast", "对比度" },
            { "unify.env_strength", "环境光对阴影颜色的影响" },
            { "unify.blur_mask", "模糊效果遮罩" },
            { "unify.compare", "修改前对比数值" },
            { "unify.apply_material", "应用材质" },

            // ===== Non-Destructive Mode =====
            { "nondestructive.title", "非破坏模式" },
            { "nondestructive.description", "复制角色和材质。原始文件不会被修改。" },
            { "nondestructive.duplicate_only", "仅复制" },
            { "nondestructive.duplicate_unify", "复制 + 统一" },
            { "nondestructive.delete_all", "删除复制" },

            // ===== Custom Material =====
            { "custom.title", "单独材质阴影调节 ({0}个)" },
            { "custom.add_help", "点击 + 按钮添加材质" },
            { "custom.exclude_notice", "这些材质将从「统一材质属性」中排除" },
            { "custom.select_material", "(选择材质)" },
            { "custom.material", "材质" },
            { "custom.current_value", "当前值" },
            { "custom.shadow_1", "阴影 1" },
            { "custom.shadow_2", "阴影 2" },
            { "custom.shadow_3", "阴影 3" },

            // ===== Preset Section =====
            { "preset.title", "预设" },
            { "preset.management", "预设管理" },
            { "preset.refresh", "刷新" },
            { "preset.empty", "没有保存的预设。" },
            { "preset.save_current", "保存当前设置" },
            { "preset.new_name", "新预设 {0}" },
            { "preset.apply", "应用" },
            { "preset.rename", "重命名" },
            { "preset.delete", "删除" },
            { "preset.export", "导出" },
            { "preset.import", "导入" },
            { "preset.default_name", "默认设置" },
            { "preset.default_desc", "lilToon 默认值" },

            // ===== Dialogs =====
            { "dialog.result", "结果" },
            { "dialog.complete", "完成" },
            { "dialog.warning", "警告" },
            { "dialog.error", "错误" },
            { "dialog.confirm", "确认" },
            { "dialog.apply", "应用" },
            { "dialog.cancel", "取消" },
            { "dialog.close", "关闭" },
            { "dialog.select", "选择" },
            { "dialog.save", "保存" },
            { "dialog.change", "更改" },
            { "dialog.overwrite", "覆盖" },
            { "dialog.duplicate", "复制" },

            // ===== Dialog Messages =====
            { "dialog.all_materials_match", "所有材质的数值都相同。" },
            { "dialog.different_materials", "数值不同的材质: {0}个" },
            { "dialog.unify_title", "统一材质" },
            { "dialog.unify_message", "将选定的属性值应用到所有 lilToon 材质。\n此操作无法撤销。" },
            { "dialog.unify_complete", "材质属性已统一。" },
            { "dialog.duplicate_title", "复制角色" },
            { "dialog.duplicate_message", "复制角色和所有 lilToon 材质。\n\n• 创建复制的角色\n• 将材质保存到新文件夹\n• 复制角色X轴-0.5移动" },
            { "dialog.duplicate_unify_title", "复制角色并统一属性" },
            { "dialog.duplicate_unify_message", "复制角色和材质，然后应用属性。\n\n• 创建复制的角色\n• 统一材质属性\n• 复制角色X轴-0.5移动" },
            { "dialog.duplicate_unify_button", "复制并统一" },
            { "dialog.delete", "删除" },
            { "dialog.delete_all_title", "删除复制的角色和材质" },
            { "dialog.delete_all_message", "删除当前选中的复制角色和材质文件夹。\n\n此操作无法撤销。" },
            { "dialog.delete_all_complete", "删除完成！\n\n角色: {0}\n材质文件夹: {1}" },
            { "dialog.delete_avatar_complete", "角色 '{0}' 已删除。" },
            { "dialog.not_duplicated_avatar", "只能删除复制的角色(_Copy)。" },
            { "dialog.no_liltoon", "未找到使用 lilToon 着色器的对象。" },
            { "dialog.apply_complete", "亮度调节功能已应用！\n\n已应用的功能（对象数量）:\n{0}" },

            // ===== Preset Dialogs =====
            { "preset.save_title", "保存新预设" },
            { "preset.name_label", "名称" },
            { "preset.desc_label", "描述" },
            { "preset.name_required", "请输入预设名称。" },
            { "preset.rename_title", "重命名预设" },
            { "preset.new_name_label", "新名称" },
            { "preset.overwrite_title", "覆盖预设" },
            { "preset.overwrite_message", "预设 '{0}' 已存在。\n是否覆盖？" },
            { "preset.save_complete_title", "保存完成" },
            { "preset.save_complete_message", "预设 '{0}' 已保存。" },
            { "preset.applied_log", "[Preset] 已应用预设 '{0}' (单独材质: {1}个)" },
            { "preset.delete_title", "删除预设" },
            { "preset.delete_message", "是否删除预设 '{0}'？\n此操作无法撤销。" },
            { "preset.export_select", "请先选择要导出的预设。" },
            { "preset.export_title", "导出预设" },
            { "preset.export_complete_title", "导出完成" },
            { "preset.export_complete_message", "预设已保存:\n{0}" },
            { "preset.import_title", "导入预设" },
            { "preset.import_complete_title", "导入完成" },
            { "preset.import_complete_message", "预设 '{0}' 已导入。" },

            // ===== Buttons =====
            { "button.apply", "应用" },
            { "button.select_avatar", "请选择角色" },
            { "button.select_feature", "请至少选择一个功能" },

            // ===== Update =====
            { "update.available", "→ v{0} 可更新 (检查VCC)" },

            // ===== Language =====
            { "language.title", "语言" },
        };
    }
}
