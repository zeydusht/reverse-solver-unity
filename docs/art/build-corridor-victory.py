"""Current corridor builder. The complete V1 is preserved under docs/art/backups/."""
from pathlib import Path
import runpy
runpy.run_path(str(Path(__file__).with_name('build-corridor-victory-v2.py')), run_name='__main__')
