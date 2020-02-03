import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductServiceImportDialogComponent } from './product-service-import-dialog.component';

describe('ProductServiceImportDialogComponent', () => {
  let component: ProductServiceImportDialogComponent;
  let fixture: ComponentFixture<ProductServiceImportDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductServiceImportDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductServiceImportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
