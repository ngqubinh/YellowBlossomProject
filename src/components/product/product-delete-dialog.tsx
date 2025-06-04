import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface DeleteProductDialogProps {
  productName: string;
  onDelete: () => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function DeleteProductDialog({ 
  productName,
  onDelete,
  loading,
  onOpenChange,
}: DeleteProductDialogProps) {
  
  const handleDelete = async () => {
    try {
      await onDelete();
      onOpenChange(false); // Close dialog when successful
    } catch (error) {
      console.error("Error deleting product:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Xóa sản phẩm</DialogTitle>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa sản phẩm "{productName}"? Hành động này không thể hoàn tác.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={handleDelete}
            disabled={loading}
          >
            {loading ? "Đang xóa..." : "Xóa"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
