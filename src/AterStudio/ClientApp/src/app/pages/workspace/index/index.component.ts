import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityFile } from 'src/app/share/models/entity/entity-file.model';
import { GenerateDto } from 'src/app/share/models/entity/generate-dto.model';
import { CommandType } from 'src/app/share/models/enum/command-type.model';
import { RequestLibType } from 'src/app/share/models/enum/request-lib-type.model';
import { Project } from 'src/app/share/models/project/project.model';
import { EntityService } from 'src/app/share/services/entity.service';
import { ProjectService } from 'src/app/share/services/project.service';
@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
  RequestLibType = RequestLibType;
  CommandType = CommandType;
  project = {} as Project;
  projectId: string;
  entityFiles = [] as EntityFile[];
  baseEntityPath = '';
  columns: string[] = ['select', 'name', 'path', 'actions'];
  dataSource!: MatTableDataSource<EntityFile>;
  isLoading = true;
  requestForm!: FormGroup;
  dialogRef!: MatDialogRef<{}, any>;
  searchKey = '';
  isSync = false;
  isListening = false;
  @ViewChild("requestDialog", { static: true })
  requestTmpRef!: TemplateRef<{}>;
  @ViewChild("syncDialog", { static: true })
  syncTmpRef!: TemplateRef<{}>;
  selection = new SelectionModel<EntityFile>(true, []);
  constructor(
    public route: ActivatedRoute,
    public router: Router,
    public service: EntityService,
    public projectSrv: ProjectService,
    public dialog: MatDialog,
    public snb: MatSnackBar
  ) {

    this.projectId = '';
    this.route.paramMap.subscribe(res => {
      this.projectId = res.get('id') ?? '';
    })
  }
  ngOnInit(): void {
    this.initForm();
    this.getProjectInfo();
    this.getEntity();
    this.getWatchStatus();
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  toggleAllRows() {
    if (this.isAllSelected()) {
      this.selection.clear();
      return;
    }
    this.selection.select(...this.dataSource.data);
  }

  getProjectInfo(): void {
    this.projectSrv.project(this.projectId)
      .subscribe(res => {
        if (res) {
          this.project = res;
          this.requestForm.get('swagger')?.setValue(res.swaggerPath);
          this.requestForm.get('path')?.setValue(res.webAppPath);
        }
      });
  }
  getWatchStatus(): void {
    this.projectSrv.getWatcherStatus(this.projectId)
      .subscribe(res => {
        if (res) {
          this.isListening = true;
        } else {
          this.isListening = false;
        }
      })
  }
  initForm(): void {
    this.requestForm = new FormGroup({
      swagger: new FormControl<string | null>('./swagger.json', []),
      type: new FormControl<RequestLibType>(RequestLibType.NgHttp, []),
      path: new FormControl<string | null>(null, [Validators.required])
    });
  }

  getEntity(): void {
    this.service.list(this.projectId!, this.searchKey)
      .subscribe(res => {
        if (res.length > 0) {
          this.entityFiles = res;
          this.baseEntityPath = res[0].baseDirPath ?? '';
          this.dataSource = new MatTableDataSource<EntityFile>(this.entityFiles);
        }
        this.isLoading = false;
      })
  }

  edit(path: string): void {

  }
  openRequestDialog(): void {
    this.dialogRef = this.dialog.open(this.requestTmpRef, {
      minWidth: 400
    });
  }
  openSyncDialog(): void {
    this.dialogRef = this.dialog.open(this.syncTmpRef, {
      minWidth: 300
    });
  }

  generate(path: string, type: CommandType): void {
    const dto: GenerateDto = {
      projectId: this.projectId!,
      entityPath: this.baseEntityPath + path,
      commandType: type
    };
    this.service.generate(dto)
      .subscribe(res => {
        if (res) {
          this.snb.open('生成成功');
        }
      })
  }

  batch(type: CommandType): void {
    const selected = this.selection.selected;
    console.log(selected);

    if (selected.length > 0) {
      this.service.batchGenerate({
        projectId: this.projectId!,
        entityPaths: selected.map(s => this.baseEntityPath + s.path),
        commandType: type
      }).subscribe(res => {
        if (res) {
          this.snb.open('生成成功');
        }
      })
    } else {
      this.snb.open('未选择任何实体');
    }
  }

  generateRequest(): void {
    this.isSync = true;
    const swagger = this.requestForm.get('swagger')?.value as string;
    const type = this.requestForm.get('type')?.value as number;
    const path = this.requestForm.get('path')?.value as string;
    this.service.generateRequest(this.projectId!, path, type, swagger)
      .subscribe({
        next: res => {
          if (res) {
            this.snb.open('生成成功');
            this.dialogRef.close();
          }
          this.isSync = false;
        },
        error:()=>{
          this.isSync = false;
        }
      })
  }

  generateSync(): void {
    this.isSync = true;
    this.service.generateSync(this.projectId!)
      .subscribe({
        next: (res) => {
          if (res) {
            this.snb.open('同步前端成功');
            this.dialogRef.close();
          }
          this.isSync = false;
        },
        error: () => {
          this.isSync = false;
        }
      })
  }

  startWatch(): void {
    this.projectSrv.startWatcher(this.projectId)
      .subscribe(res => {
        if (res) {
          this.isListening = true;
          this.snb.open('已开始监听');
        }
      })
  }

  stopWatch(): void {
    this.projectSrv.stopWatcher(this.projectId)
      .subscribe(res => {
        if (res) {
          this.isListening = false;
          this.snb.open('已停止监听');
        }
      })
  }

}
