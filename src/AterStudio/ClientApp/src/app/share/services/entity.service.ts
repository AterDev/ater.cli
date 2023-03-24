import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { Observable } from 'rxjs';
import { GenerateDto } from '../models/entity/generate-dto.model';
import { BatchGenerateDto } from '../models/entity/batch-generate-dto.model';
import { EntityFile } from '../models/entity/entity-file.model';
import { RequestLibType } from '../models/enum/request-lib-type.model';
import { LanguageType } from '../models/enum/language-type.model';

/**
 * 实体
 */
@Injectable({ providedIn: 'root' })
export class EntityService extends BaseService {
  /**
   * list
   * @param id string
   * @param name string
   */
  list(id: string, name?: string): Observable<EntityFile[]> {
    const url = `/api/Entity/${id}?name=${name}`;
    return this.request<EntityFile[]>('get', url);
  }

  /**
   * generate
   * @param data GenerateDto
   */
  generate(data: GenerateDto): Observable<boolean> {
    const url = `/api/Entity/generate`;
    return this.request<boolean>('post', url, data);
  }

  /**
   * 批量生成
   * @param data BatchGenerateDto
   */
  batchGenerate(data: BatchGenerateDto): Observable<boolean> {
    const url = `/api/Entity/batch-generate`;
    return this.request<boolean>('post', url, data);
  }

  /**
   * 生成前端请求
   * @param id 
   * @param webPath 
   * @param type 
   * @param swaggerPath 
   */
  generateRequest(id: string, webPath?: string, type?: RequestLibType, swaggerPath?: string): Observable<boolean> {
    const url = `/api/Entity/generateRequest/${id}?webPath=${webPath}&type=${type}&swaggerPath=${swaggerPath}`;
    return this.request<boolean>('get', url);
  }

  /**
   * 生成客户端请求
   * @param id 
   * @param webPath 
   * @param type 
   * @param swaggerPath 
   */
  generateClientRequest(id: string, webPath?: string, type?: LanguageType, swaggerPath?: string): Observable<boolean> {
    const url = `/api/Entity/generateClientRequest/${id}?webPath=${webPath}&type=${type}&swaggerPath=${swaggerPath}`;
    return this.request<boolean>('get', url);
  }

  /**
   * 同步ng页面
   * @param id 
   */
  generateSync(id: string): Observable<boolean> {
    const url = `/api/Entity/generateSync/${id}`;
    return this.request<boolean>('post', url);
  }

}
