/**
 * 操作执行模型
 */
export interface GenActionRunDto {
  /**
   * 操作id
   */
  id: string;
  /**
   * 源文件路径
   */
  sourceFilePath?: string | null;
  /**
   * 仅输出
   */
  onlyOutput: boolean;

}
