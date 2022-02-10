import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { {$EntityName}Service } from 'src/app/share/services/{$EntityPathName}.service';
import { {$EntityName} } from 'src/app/share/models/{$EntityPathName}/{$EntityPathName}.model';
import { {$EntityName}UpdateDto } from 'src/app/share/models/{$EntityPathName}/{$EntityPathName}-update-dto.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { Status } from 'src/app/share/models/enum/status.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Location } from '@angular/common';

@Component({
    selector: 'app-add',
    templateUrl: './add.component.html',
    styleUrls: ['./add.component.css']
})
export class AddComponent implements OnInit {

    formGroup!: FormGroup;
    data = {} as {$EntityName}UpdateDto;
    isLoading = true;
    status = Status;
    constructor(
        private service: {$EntityName}Service,
        public snb: MatSnackBar,
        private router: Router,
        private route: ActivatedRoute,
        private location: Location
        // public dialogRef: MatDialogRef<AddComponent>,
        // @Inject(MAT_DIALOG_DATA) public dlgData: { id: '' }
    ) {

    }

{$DefinedProperties}

  ngOnInit(): void {
    this.initForm();
    // TODO:获取其他相关数据后设置加载状态
    this.isLoading = false;
  }

  initForm(): void {
    this.formGroup = new FormGroup({
{$DefinedFormControls}
    });
  }
  getValidatorMessage(type: string): string {
    switch (type) {
{$DefinedValidatorMessage}
      default:
    return '';
    }
  }

  add(): void {
    if(this.formGroup.valid) {
    const data = this.formGroup.value as {$EntityName}UpdateDto;
    this.data = { ...data, ...this.data };
    this.service.add(this.data as Article)
        .subscribe(res => {
            this.snb.open('添加成功');
            // this.dialogRef.close(res);
            // this.router.navigate(['../index'],{relativeTo: this.route});
        });
    }
  }
  back(): void {
    this.location.back();
  }
  upload(event: any, type ?: string): void {
    const files = event.target.files;
    if(files[0]) {
      const formdata = new FormData();
      formdata.append('file', files[0]);
    /*    this.service.uploadFile('agent-info' + type, formdata)
          .subscribe(res => {
            this.data.logoUrl = res.url;
          }, error => {
            this.snb.open(error?.detail);
          }); */
    } else {

    }
  }
}
