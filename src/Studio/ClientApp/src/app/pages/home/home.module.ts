import { NgModule } from '@angular/core';
import { HomeRoutingModule } from './home-routing.module';
import { LoginComponent } from './login/login.component';
import { IndexComponent } from './index/index.component';
import { ShareModule } from 'src/app/share/share.module';
import { CreateComponent } from './create/create.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { MarkdownModule } from 'ngx-markdown';
import { ToKeyValuePipeModule } from 'src/app/share/pipe/to-key-value.pipe';
import { EnumTextPipe } from 'src/app/pipe/enum-text.pipe';


@NgModule({
  declarations: [
    LoginComponent,
    IndexComponent,
    CreateComponent,
  ],
  imports: [
    ShareModule,
    HomeRoutingModule,
    MatTableModule,
    MatPaginatorModule,
    EnumTextPipe,
    MatSortModule,
    MonacoEditorModule,
    ToKeyValuePipeModule,
    MarkdownModule.forRoot()
  ]
})
export class HomeModule { }
