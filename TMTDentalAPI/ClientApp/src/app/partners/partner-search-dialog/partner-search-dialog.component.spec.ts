import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSearchDialogComponent } from './partner-search-dialog.component';

describe('PartnerSearchDialogComponent', () => {
  let component: PartnerSearchDialogComponent;
  let fixture: ComponentFixture<PartnerSearchDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSearchDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSearchDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
