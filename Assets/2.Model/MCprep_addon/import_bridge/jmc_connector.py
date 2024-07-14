# ##### BEGIN GPL LICENSE BLOCK #####
#
#  This program is free software; you can redistribute it and/or
#  modify it under the terms of the GNU General Public License
#  as published by the Free Software Foundation; either version 2
#  of the License, or (at your option) any later version.
#
#  This program is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  along with this program; if not, write to the Free Software Foundation,
#  Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
#
# ##### END GPL LICENSE BLOCK #####

"""Jmc2Obj Connector

Pure-python wrapper for running and managing Mienways exports.
"""

import os
import platform
from subprocess import Popen, PIPE


class JmcConnector(object):
	"""Pure python bridge class for calling and controlling Jmc2Obj"""

	def __init__(self, exec_path, saves, open_ui=False):
		self.exec_path = exec_path # path to mineways executable
		self.saves_path = saves # path to folder of world saves (directories)
		self.open_ui = open_ui # If true, opens UI; if not, minimized & closes
		self.world = None  # folder name of save, child of saves_path above
		self.layer = None  # one of Overworld, Nether, The End

	def set_world(self, world, layer='Overworld'):
		"""Set the world save and region."""
		if layer not in ['Overworld', 'Nether', 'The End']:
			raise Exception("layer")
		self.world = world
		self.layer = layer

	def run_command(self, arg_list):
		"""Opens Jmc2Obj in command line headless mode to run command"""

		if platform.system() == "Darwin": # ie OSX
			cmd = ['java', '-jar', self.exec_path]
		else:
			cmd = ['java', '-jar', self.exec_path]

		cmd += arg_list

		p = Popen(cmd, stdin=PIPE, stdout=PIPE, stderr=PIPE)
		stdout, err = p.communicate(b"")
		print(str(stdout))

		if err != b"":
			return "Error occured while running command: "+str(err)
		return [False, []]

	def list_worlds(self):
		"""Get the list of MC worlds on this machine"""
		worlds = [fold for fold in os.listdir(self.saves_path)
			if os.path.isdir(os.path.join(self.saves_path, fold))]
		return worlds

	def default_mcprep_obj(self):
		"""Decent default commands to set for output"""
		cmds = [
			"--render-entities",
			"--object-per-mat"
		]
		return cmds

	def run_export_multiple(self, export_path, coord_list):
		"""Run mineways export based on world name and coordinates.

		Arguments:
			world: Name of the world matching folder in save folder
			min_corner: First coordinate for volume
			max_corner: Second coordinate for volume
		Returns:
			List of intended obj files, may not exist yet
		"""
		cmds = []

		# Add some default good to have settings
		cmds += self.default_mcprep_obj()

		# Set world layer to export (Jmc2Obj default is overworld)
		if self.layer == "Overworld":
			cmds.append("--dimension=0")
		elif self.layer == "Nether":
			cmds.append("--dimension=-1")
		elif self.layer == "The End":
			cmds.append("--dimension=1")

		root_path = os.path.dirname(export_path)
		root_name, _ = os.path.splitext(os.path.basename(export_path))
		cmds.append('--output='+root_path)

		# will result in multiple calls to Jmc2Obj for each set of exports
		for coord_a, coord_b in coord_list:
			if len(coord_a) != 3 or len(coord_b) != 3:
				raise Exception("Coordinates must be length 3")
			for point in coord_a+coord_b:
				if not isinstance(point, int):
					raise Exception("Coordinates must be integers")

			this_cmd = list(cmds)

			minx=min(coord_a[0], coord_b[0])
			minz=min(coord_a[2], coord_b[2])
			maxx=max(coord_a[0], coord_b[0])
			maxz=max(coord_a[2], coord_b[2])
			miny=min(coord_a[1], coord_b[1])
			maxy=max(coord_a[1], coord_b[1])

			this_cmd.append("--area={minx},{minz},{maxx},{maxz}".format(
				minx=minx,minz=minz,maxx=maxx,maxz=maxz))
			this_cmd.append("--height={miny},{maxy}".format(
				miny=miny, maxy=maxy))
			exp_id = "{minx}-{miny}-{minz}-{maxx}-{maxy}-{maxz}".format(
				minx=minx,minz=minz,miny=miny,maxy=maxy,maxx=maxx,maxz=maxz)

			this_cmd.append('--export=obj,mtl') # assumes to NOT export tex

			this_cmd.append('--objfile={}-{}.obj'.format(root_name, exp_id))
			this_cmd.append('--mtlfile={}-{}.mtl'.format(root_name, exp_id))

			# finally, append the world path arg
			# this_cmd.append(
			# 	'"' + os.path.join(self.saves_path, self.world)+ '"')
			this_cmd.append(
				os.path.join(self.saves_path, self.world))
			print("Running JMC with commands:")
			print(this_cmd)
			ok, err = self.run_command(this_cmd)
			if not ok and "is not a valid directory" in err:
				"Jmc2Obj could not find save directory/world specified"

		return False

def run_test():
	"""Run default test open and export."""

	exec_path = '/Users/patrickcrawford/Documents/blender/minecraft/jmc2obj/jMc2Obj-107.jar'
	saves_path = '/Users/patrickcrawford/Library/Application Support/minecraft/saves/'

	connector = JmcConnector(exec_path, saves_path)

	print("Running Jmc2Obj - MCprep bridge test")
	worlds = connector.list_worlds()
	print(worlds)

	world = "WorldGen 14_3" # "QMAGNET's Test Map [1.12.1] mod"
	print("using hard-coded world: ", world)
	coord_a = [-40, 0, -40]
	coord_b = [40, 255, 40]

	# takes single set of coordinate inputs,
	# the multi version would have a list of 2-coords
	print("Running export")
	obj_path = '/Users/patrickcrawford/Desktop/temp/out_file_test.obj'
	connector.set_world(world)
	connector.run_export_single(obj_path, coord_a, coord_b)


if __name__ == "__main__":
	run_test()
