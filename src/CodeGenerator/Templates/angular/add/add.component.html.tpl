<mat-toolbar class="d-flex gap-1">
  <!-- <mat-icon class="clickable" matTooltip="返回" (click)="back()">arrow_back</mat-icon> -->
  <span>添加</span>
</mat-toolbar>
<form class="d-flex p-2" *ngIf="!isLoading" [formGroup]="formGroup">
  <div class="d-flex flex-column w-100">
#@FormControls#
    <div class="d-flex mt-1 justify-content-end">
      <button class="d-flex flex-column" mat-flat-button color="primary" (click)="add()">添加</button>
    </div>
  </div>
</form>

