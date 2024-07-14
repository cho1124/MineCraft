import bpy

class MCPREP_PT_wold(bpy.types.Panel):
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'TOOLS' if bpy.app.version < (2,80) else 'UI'
    bl_category = "Wolf"
    bl_label = "Wolf Properties"

    @classmethod
    def poll(self, context):
        try:
            return (context.active_object.data.get("rig_name") == "wolfRig")
        except (AttributeError, KeyError, TypeError):
            return False

    def draw(self, context):
        layout = self.layout
        col = layout.column()
        pose_bones = context.active_object.pose.bones

        col.label(text="Trainguy's Wolf Rig v2")
        col.prop(pose_bones["Properties"], '["mad"]', text="Enable Red Eyes", slider=True)
        col.prop(pose_bones["Properties"], '["collar"]', text="Enable Collar", slider=True)

bpy.utils.register_class(MCPREP_PT_wold)
