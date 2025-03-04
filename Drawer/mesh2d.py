import matplotlib.pyplot as plt
import numpy as np

x = [x for x in range(0, 3, 1)]

y = [y for y in range(0, 3, 1)]

for xi in x:
    plt.plot([xi, xi], [max(y), min(y)], 'black', linewidth = 0.5)
for yi in y:
    plt.plot([min(x), max(x)], [yi, yi], 'black', linewidth = 0.5)



plt.plot([min(x), max(x)], [0.0, 0.0], 'black', linewidth = 0.5)
plt.plot([0.0, 2], [2, 2], color = 'black', linewidth=2.0)
plt.plot([0.0, 0.0], [0.0, 2], color = 'black', linewidth=2.0)
plt.plot([2, 2], [0.0, 2], color = 'black', linewidth=2.0)
plt.plot([0.0, 2], [0.0, 0.0], color = 'black', linewidth=2.0)
plt.scatter(0.5, 0.5, color = 'red', linewidth=2.0)
plt.scatter(1.5, 0.5, color = 'red', linewidth=2.0)
plt.scatter(0.5, 1.5, color = 'red', linewidth=2.0)
plt.scatter(1.5, 1.5, color = 'red', linewidth=2.0)
plt.legend()
plt.xlabel("X")
plt.ylabel("Y")
plt.show()