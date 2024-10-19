import { Variable } from '../../models/variable.model';
import { GenSourceType } from '../../enum/models/gen-source-type.model';
import { Project } from '../../project/models/project.model';
/**
 * The project's generate action详情
 */
export interface GenActionDetailDto {
  /**
   * action name
   */
  name: string;
  description?: string | null;
  /**
   * 实体路径
   */
  entityPath?: string | null;
  /**
   * open api path
   */
  openApiPath?: string | null;
  variables?: Variable[];
  sourceType?: GenSourceType | null;
  project?: Project | null;
  projectId: string;
  id: string;
  createdTime: Date;
  updatedTime: Date;

}
