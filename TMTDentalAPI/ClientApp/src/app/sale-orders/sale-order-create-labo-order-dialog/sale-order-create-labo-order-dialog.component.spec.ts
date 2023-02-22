import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderCreateLaboOrderDialogComponent } from './sale-order-create-labo-order-dialog.component';

describe('SaleOrderCreateLaboOrderDialogComponent', () => {
  let component: SaleOrderCreateLaboOrderDialogComponent;
  let fixture: ComponentFixture<SaleOrderCreateLaboOrderDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderCreateLaboOrderDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderCreateLaboOrderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
