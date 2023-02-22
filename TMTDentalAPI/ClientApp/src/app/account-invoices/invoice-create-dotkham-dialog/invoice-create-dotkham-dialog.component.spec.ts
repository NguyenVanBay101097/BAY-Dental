import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceCreateDotkhamDialogComponent } from './invoice-create-dotkham-dialog.component';

describe('InvoiceCreateDotkhamDialogComponent', () => {
  let component: InvoiceCreateDotkhamDialogComponent;
  let fixture: ComponentFixture<InvoiceCreateDotkhamDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceCreateDotkhamDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceCreateDotkhamDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
