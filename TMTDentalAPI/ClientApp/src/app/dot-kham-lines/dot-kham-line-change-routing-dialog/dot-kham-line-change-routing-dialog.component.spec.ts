import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamLineChangeRoutingDialogComponent } from './dot-kham-line-change-routing-dialog.component';

describe('DotKhamLineChangeRoutingDialogComponent', () => {
  let component: DotKhamLineChangeRoutingDialogComponent;
  let fixture: ComponentFixture<DotKhamLineChangeRoutingDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamLineChangeRoutingDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamLineChangeRoutingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
