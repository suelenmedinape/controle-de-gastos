enum TableHeaders {
  person = 1,
  category = 2,
  transaction = 3,
}

export interface TableHeadersProps {
  isByPerson: TableHeaders;
}

export interface TableHeadersPropsWithModal extends TableHeadersProps {
  onOpenModal: () => void;
}