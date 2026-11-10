"""Current corridor validation; V2 baseline checker is preserved with the backup."""
from pathlib import Path
import runpy
runpy.run_path(str(Path(__file__).with_name('check-corridor-continuity.py')), run_name='__main__')
