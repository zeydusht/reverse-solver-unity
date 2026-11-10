"""Current corridor asset builder; the previous implementation is preserved in docs/art/backups/."""
from pathlib import Path
import runpy
runpy.run_path(str(Path(__file__).with_name('build-corridor-continuity.py')), run_name='__main__')
