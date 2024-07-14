import bpy
import math
from math import pi
from mathutils import Vector

rig_id = "w8r2wunlfe48a726"


############################
## Math utility functions ##
############################

def perpendicular_vector(v):
    """ Returns a vector that is perpendicular to the one given.
        The returned vector is _not_ guaranteed to be normalized.
    """
    # Create a vector that is not aligned with v.
    # It doesn't matter what vector.  Just any vector
    # that's guaranteed to not be pointing in the same
    # direction.
    if abs(v[0]) < abs(v[1]):
        tv = Vector((1,0,0))
    else:
        tv = Vector((0,1,0))

    # Use cross prouct to generate a vector perpendicular to
    # both tv and (more importantly) v.
    return v.cross(tv)


def rotation_difference(mat1, mat2):
    """ Returns the shortest-path rotational difference between two
        matrices.
    """
    q1 = mat1.to_quaternion()
    q2 = mat2.to_quaternion()
    angle = math.acos(min(1,max(-1,q1.dot(q2)))) * 2
    if angle > pi:
        angle = -angle + (2*pi)
    return angle

def find_min_range(f,start_angle,delta=pi/8):
    """ finds the range where lies the minimum of function f applied on bone_ik and bone_fk
        at a certain angle.
    """
    angle = start_angle
    while (angle > (start_angle - 2*pi)) and (angle < (start_angle + 2*pi)):
        l_dist = f(angle-delta)
        c_dist = f(angle)
        r_dist = f(angle+delta)
        if min((l_dist,c_dist,r_dist)) == c_dist:
            return (angle-delta,angle+delta)
        else:
            angle=angle+delta

def ternarySearch(f, left, right, absolutePrecision):
    """
    Find minimum of unimodal function f() within [left, right]
    To find the maximum, revert the if/else statement or revert the comparison.
    """
    while True:
        #left and right are the current bounds; the maximum is between them
        if abs(right - left) < absolutePrecision:
            return (left + right)/2

        leftThird = left + (right - left)/3
        rightThird = right - (right - left)/3

        if f(leftThird) > f(rightThird):
            left = leftThird
        else:
            right = rightThird

###################
## Rig UI Panels ##
###################

class RigUI(bpy.types.Panel):
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_label = "Rig Main Properties"
    bl_idname = "VIEW3D_PT_rig_ui_" + rig_id
    bl_category = 'Item'

    @classmethod
    def poll(self, context):
        if context.mode != 'POSE':
            return False
        try:
            return (context.active_object.data.get("rig_id") == rig_id)
        except (AttributeError, KeyError, TypeError):
            return False

    def draw(self, context):
        layout = self.layout
        pose_bones = context.active_object.pose.bones
        try:
            selected_bones = set(bone.name for bone in context.selected_pose_bones)
            selected_bones.add(context.active_pose_bone.name)
        except (AttributeError, TypeError):
            return

        def is_selected(names):
            # Returns whether any of the named bones are selected.
            if isinstance(names, list) or isinstance(names, set):
                return not selected_bones.isdisjoint(names)
            elif names in selected_bones:
                return True
            return False

        num_rig_separators = [-1]

        def emit_rig_separator():
            if num_rig_separators[0] >= 0:
                layout.separator()
            num_rig_separators[0] += 1
            
        col = layout.column()
        col.prop(pose_bones['Croaking.Ctrl'], '["variant"]', text='Frog Variant', slider=True)
        

class RigLayers(bpy.types.Panel):
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_label = "Rig Layers"
    bl_idname = "VIEW3D_PT_rig_layers_" + rig_id
    bl_category = 'Item'

    @classmethod
    def poll(self, context):
        try:
            return (context.active_object.data.get("rig_id") == rig_id)
        except (AttributeError, KeyError, TypeError):
            return False

    def draw(self, context):
        layout = self.layout
        col = layout.column()
        row = col.row()
        row.prop(context.active_object.data, 'layers', index=2, toggle=True, text='Left')
        row.prop(context.active_object.data, 'layers', index=4, toggle=True, text='Right')

        row = col.row()
        row.prop(context.active_object.data, 'layers', index=1, toggle=True, text='Tongue')

        row = col.row()
        row.prop(context.active_object.data, 'layers', index=0, toggle=True, text='Body')

        row = col.row()
        row.separator()
        row = col.row()
        row.separator()
        
        row = col.row()
        row.prop(context.active_object.data, 'layers', index=28, toggle=True, text='Root')


def register():
    bpy.utils.register_class(RigUI)
    bpy.utils.register_class(RigLayers)

def unregister():
    bpy.utils.unregister_class(RigUI)
    bpy.utils.unregister_class(RigLayers)

register()