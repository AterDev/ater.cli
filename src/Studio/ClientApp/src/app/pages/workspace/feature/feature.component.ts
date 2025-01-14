import { Component, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Project } from 'src/app/services/project/models/project.model';
import { ProjectStateService } from 'src/app/share/project-state.service';
import { SolutionService } from 'src/app/services/solution/solution.service';
import { SubProjectInfo } from 'src/app/services/solution/models/sub-project-info.model';

@Component({
    selector: 'app-feature',
    templateUrl: './feature.component.html',
    styleUrls: ['./feature.component.css'],
    standalone: false
})
export class FeatureComponent {
  @ViewChild(MatPaginator, { static: true }) paginator!: MatPaginator;
  isLoading = true;
  isProcessing = false;
  total = 0;
  data: SubProjectInfo[] = [];
  columns: string[] = ['name', 'path', 'actions'];
  dataSource!: MatTableDataSource<SubProjectInfo>;
  dialogRef!: MatDialogRef<{}, any>;
  @ViewChild('addModuleDialog', { static: true })
  addTmpl!: TemplateRef<{}>;
  mydialogForm!: FormGroup;

  project: Project | null = null;
  pageSizeOption = [12, 20, 50];
  constructor(
    private service: SolutionService,
    private projectSrv: ProjectStateService,
    private snb: MatSnackBar,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.project = this.projectSrv.project;
  }

  get name() { return this.mydialogForm.get('name'); }

  ngOnInit(): void {
    this.getList();
    this.mydialogForm = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.maxLength(40)]),
    })
  }

  getList(event?: PageEvent): void {
    this.service.getModulesInfo()
      .subscribe({
        next: (res) => {
          if (res) {
            this.data = res;
            this.dataSource = new MatTableDataSource<SubProjectInfo>(this.data);

          } else {
            this.snb.open('');
          }
        },
        error: (error) => {
          this.snb.open(error.detail);
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        }
      });
  }

  openModuleDialog(): void {
    this.dialogRef = this.dialog.open(this.addTmpl, {
      width: '300px',
    });
  }

  addModule(): void {
    if (this.mydialogForm.valid) {
      this.isProcessing = true;
      this.service.createModule(this.name?.value)
        .subscribe({
          next: (res) => {
            if (res) {

              this.snb.open('添加成功');
              this.getList();
            } else {
            }
          },
          error: (error) => {
            this.snb.open(error.detail);
            this.isProcessing = false;
          },
          complete: () => {
            this.dialogRef.close();
            this.isProcessing = false;
          }
        });
    }
  }
}
