import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCategoryCuDialogComponent } from './partner-category-cu-dialog.component';

describe('PartnerCategoryCuDialogComponent', () => {
  let component: PartnerCategoryCuDialogComponent;
  let fixture: ComponentFixture<PartnerCategoryCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCategoryCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCategoryCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
