import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectUomProductDialogComponent } from './select-uom-product-dialog.component';

describe('SelectUomProductDialogComponent', () => {
  let component: SelectUomProductDialogComponent;
  let fixture: ComponentFixture<SelectUomProductDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectUomProductDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectUomProductDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
