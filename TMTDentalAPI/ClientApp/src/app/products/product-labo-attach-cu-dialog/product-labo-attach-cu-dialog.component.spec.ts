import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductLaboAttachCuDialogComponent } from './product-labo-attach-cu-dialog.component';

describe('ProductLaboAttachCuDialogComponent', () => {
  let component: ProductLaboAttachCuDialogComponent;
  let fixture: ComponentFixture<ProductLaboAttachCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductLaboAttachCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductLaboAttachCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
