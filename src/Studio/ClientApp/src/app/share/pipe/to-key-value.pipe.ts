import { NgModule, Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'toKeyValue',
    standalone: false
})
export class ToKeyValuePipe implements PipeTransform {
  transform(enumData: any): { key: string, value: number }[] {

    var result = Object.keys(enumData)
      .filter(key => isNaN(Number(key)))
      .map((key) => ({ key, value: enumData[key] }));

    return result;
  }
}

@NgModule({
  declarations: [ToKeyValuePipe], exports: [ToKeyValuePipe]
})
export class ToKeyValuePipeModule { }