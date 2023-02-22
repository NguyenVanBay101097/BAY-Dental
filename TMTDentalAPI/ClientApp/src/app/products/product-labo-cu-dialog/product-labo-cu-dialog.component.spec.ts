import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductLaboCuDialogComponent } from './product-labo-cu-dialog.component';

describe('ProductLaboCuDialogComponent', () => {
  let component: ProductLaboCuDialogComponent;
  let fixture: ComponentFixture<ProductLaboCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductLaboCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductLaboCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
