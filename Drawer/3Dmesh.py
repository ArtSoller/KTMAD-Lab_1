import matplotlib.pyplot as plt
import numpy as np
from mpl_toolkits.mplot3d import Axes3D


ax = plt.figure(figsize=(19.80, 10.80)).add_subplot(projection='3d')

xarr = [0.0, 1.0, 2.0]
yarr = [0.0, 1.0, 2.0]
zarr = [0.0, 1.0, 2.0]

for k in range(len(zarr)):
    for j in range(len(yarr)):
        for i in range(len(xarr) - 1):
            ax.plot([xarr[i], xarr[i + 1]], [yarr[j], yarr[j]], [zarr[k], zarr[k]], color = "black", linewidth = 2.0)

for k in range(len(zarr)):
    for j in range(len(yarr) - 1):
        for i in range(len(xarr)):
            ax.plot([xarr[i], xarr[i]], [yarr[j], yarr[j + 1]], [zarr[k], zarr[k]], color = "black", linewidth = 2.0)
            
for k in range(len(zarr) - 1):
    for j in range(len(yarr)):
        for i in range(len(xarr)):
            ax.plot([xarr[i], xarr[i]], [yarr[j], yarr[j]], [zarr[k], zarr[k + 1]], color = "black", linewidth = 2.0)
            
ax.xaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
ax.yaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
ax.zaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
ax.grid(visible=False)

ax.legend()
ax.set(
    xlabel='X',
    ylabel='Y',
    zlabel='Z')
#plt.savefig('D:\\CodeRepos\\Diplom\\Docs\\Производственная практика\\images\\3D_test_mesh.png')
plt.show()