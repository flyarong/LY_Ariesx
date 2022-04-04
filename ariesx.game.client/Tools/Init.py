import os
import shutil

assets_path = os.path.abspath(os.path.pardir) + "/Assets"
tool_path = os.path.abspath(os.path.pardir)  + "/Tools/"

#Generate Debug.dll
unityEngine = tool_path + "UnityEngine.dll"
 #unityEditor = tool_path + "UnityEditor.dll"
plugin_path = assets_path + "/Plugins/Other/poukoute"
mcs = tool_path + "mcs"

# debug = tool_path + "Debug.cs"
# cmd = mcs + " -r:" + unityEngine + " -target:library -sdk:2 " + debug
# os.system(cmd)
# src = tool_path + "Debug.dll"
# dst = plugin_path
# shutil.copy(src, dst)
# os.remove(src)


protocol_path = os.path.abspath(os.path.pardir) + "/ariesx.proto.v2/"
protoGen = tool_path + "protogen"
#Generate Protocol.pb
# target_path = assets_path + "/Scripts/Lua/Protocol/Protocol.pb"
# ack= "ack.proto"
# api = "api.proto"
# request = "request.proto"
# acknowledge = "acknowledge.proto"
# struct = "struct.proto"
# notify = "notify.proto"
# os.chdir(protocol_path)
# cmd = "protoc" + " " + ack + " " + api + " " + request + " " + acknowledge + " " + struct + " " + notify + " -o " + target_path
# os.system(cmd)

#Generate Protocol.cs
target_path = assets_path + "/Scripts/CSharp/Net/Protocol.cs"
ack= protocol_path + "ack.proto"
api = protocol_path + "api.proto"
request = protocol_path + "request.proto"
acknowledge = protocol_path + "acknowledge.proto"
battle = protocol_path + "battle_report.proto"
struct = protocol_path + "struct.proto"
models = protocol_path + "models.proto"
static_map = protocol_path + "static_map.proto"
notify = protocol_path + "notify.proto";
types = protocol_path + "types.proto";
cmd = protoGen + " -i:" + ack + " -i:" + api + " -i:" + static_map + " -i:" + request + " -i:" + models + " -i:" + \
    acknowledge + " -i:" + struct + " -i:" + battle + " -i:" + notify + " -i:" + types + " -o:" + target_path + " -p:fixCase"
os.system(cmd)
