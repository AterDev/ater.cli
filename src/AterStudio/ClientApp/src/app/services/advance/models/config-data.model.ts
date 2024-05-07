import { ValueType } from '../../enum/models/value-type.model';
import { Project } from '../../project/models/project.model';
/**
 * 配置
 */
export interface ConfigData {
  id: string;
  createdTime: Date;
  updatedTime: Date;
  isDeleted: boolean;
  /**
   * 键
   */
  key: string;
  /**
   * 值
   */
  value: string;
  valueType?: ValueType | null;
  /**
   * 项目
   */
  project?: Project | null;
  projectId: string;

}