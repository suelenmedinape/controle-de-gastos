export interface ModalNewPersonProps {
  isOpen: boolean;
  onClose: () => void;
  onPersonAdded?: () => void;
  isPerson?: boolean;
}