import type { PersonModel } from "./person-model";

export interface ModalNewPersonProps {
  isOpen: boolean;
  onClose: () => void;
  onPersonAdded?: () => void;
  isPerson?: boolean;
}

export interface ModalNewTransactionProps {
  isOpen: boolean;
  onClose: () => void;
  onTransactionAdded?: () => void;
}

export interface ModalEditPersonProps {
  isOpen: boolean;
  onClose: () => void;
  onPersonUpdated?: () => void;
  person: PersonModel | null;
}