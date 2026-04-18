export interface TaskItem {
  id: string;
  title: string;
  description: string | null;
  status: string;
  projectId: string;
}